using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;


namespace Reciplease.Controllers;

[ApiController, Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<UserController> _logger;
    private readonly RecipleaseContext _context;

    public UserController(
        ILogger<UserController> logger, 
        RecipleaseContext context,
        UserManager<IdentityUser> userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }

    public class AuthenticateRequest
    {
        [Required]
        public string IdToken { get; set; }
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var userExist = await _userManager.FindByNameAsync(model.Username);
        if (userExist != null)
            return StatusCode(500);

        IdentityUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username
            
        };
        await _userManager.AddToRoleAsync(user, "User");
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return StatusCode(500);
        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("authenticate")]
    public IActionResult Authenticate([FromBody] AuthenticateRequest data)
    {
        GoogleJsonWebSignature.ValidationSettings settings = new GoogleJsonWebSignature.ValidationSettings();
        
        settings.Audience = new List<string>() {_context.googleClientId };

        GoogleJsonWebSignature.Payload payload = GoogleJsonWebSignature.ValidateAsync(data.IdToken, settings).Result;
        return Ok( payload );
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = GetToken(authClaims);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }

        return Unauthorized();
    }

    private JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_context.JwtSecret));

        var token = new JwtSecurityToken(
            issuer: _context.JwtValidIssuer,
            audience: _context.JwtValidAudience,
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
        return token;
    }
}
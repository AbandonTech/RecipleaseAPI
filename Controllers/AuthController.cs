using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Reciplease.Controllers;

[ApiController, Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<UserController> _logger;
    private readonly RecipleaseContext _context;

    public AuthController(
        ILogger<UserController> logger, 
        RecipleaseContext context,
        UserManager<IdentityUser> userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }
    
    [HttpPost("authenticate")]
    public IActionResult Authenticate([FromBody] AuthenticateThirdPartyDto data)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings();
        
        settings.Audience = new List<string>() {_context.googleClientId };

        var payload = GoogleJsonWebSignature.ValidateAsync(data.IdToken, settings).Result;
        return Ok( payload );
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginInputDto inputDto)
    {
        var user = await _userManager.FindByNameAsync(inputDto.Username);

        if (user == null || !await _userManager.CheckPasswordAsync(user, inputDto.Password))
        {
            return Unauthorized("User name or password invalid.");
        }

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
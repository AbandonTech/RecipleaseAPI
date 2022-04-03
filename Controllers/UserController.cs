using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Reciplease.Controllers;

[Authorize]
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

    [AllowAnonymous]
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var userExist = await _userManager.FindByNameAsync(model.Username);
        if (userExist != null)
            return Conflict("A user with this user name already exists.");

        IdentityUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username
            
        };
        // await _userManager.AddToRoleAsync(user, "User"); // Placeholder for when we add user roles
        
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return StatusCode(500, result.Errors);
        return CreatedAtAction(nameof(Profile), new {Username = user.UserName}, user);
    }

    [HttpGet]
    [Route("profile")]
    public async Task<ActionResult<UserDto>> Profile([FromQuery] UserInputDto user)
    {
        var result = await _userManager.FindByNameAsync(user.Username);
        return new UserDto {Username = result.UserName, Email = result.Email};
    }
}
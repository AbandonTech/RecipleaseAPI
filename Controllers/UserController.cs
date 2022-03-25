
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Reciplease.Controllers;

[ApiController, Route("api/[controller]")]
public class UserController : ControllerBase
{
     private readonly ILogger<UserController> _logger;
    private readonly RecipleaseContext _context;
    private readonly JwtGenerator _jwtGenerator;

    public UserController(ILogger<UserController> logger, RecipleaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    public class AuthenticateRequest
    {
        [Required]
        public string IdToken { get; set; }
    }

    [AllowAnonymous]
    [HttpPost("authenticate")]
    public IActionResult Authenticate([FromBody] AuthenticateRequest data)
    {
        GoogleJsonWebSignature.ValidationSettings settings = new GoogleJsonWebSignature.ValidationSettings();
        
        settings.Audience = new List<string>() {_context.googleClientId };

        GoogleJsonWebSignature.Payload payload = GoogleJsonWebSignature.ValidateAsync(data.IdToken, settings).Result;
        return Ok(new { AuthToken = _jwtGenerator.CreateUserAuthToken(payload.Email) });
    }
}
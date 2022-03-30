using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Reciplease;

public class User
{
    public int Id { get; set; }
    [Required] public string Username { get; set; }

    // Scope has default value of "User" defined in RecipleaseContext.cs
    [Required] public string Scope { get; set; }
}

public class ExternalAuthDto
{
    public string Provider { get; set; }
    public string IdToken { get; set; }
}

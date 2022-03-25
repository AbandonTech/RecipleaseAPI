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

public class JwtGenerator
{
    readonly RsaSecurityKey _key;
    public JwtGenerator(string signingKey)
    {
        RSA privateRSA = RSA.Create();
        privateRSA.ImportRSAPrivateKey(Convert.FromBase64String(signingKey), out _);
        _key = new RsaSecurityKey(privateRSA);
    }

    public string CreateUserAuthToken(string userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = "myApi",
            Issuer = "AuthService",
            Subject = new ClaimsIdentity(new Claim[]
            {
                        new Claim(ClaimTypes.Sid, userId.ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(60),
            SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.RsaSha256)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
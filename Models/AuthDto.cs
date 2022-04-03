using System.ComponentModel.DataAnnotations;

namespace Reciplease;

public class AuthenticateThirdPartyDto
{
    [Required]
    public string IdToken { get; set; }
}

public class LoginInputDto
{
    [Required(ErrorMessage = "User Name is required")]
    public string? Username { get; set; }
    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }
}
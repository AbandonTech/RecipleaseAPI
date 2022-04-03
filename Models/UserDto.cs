using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Reciplease;

public class UserInputDto
{
    [FromQuery]
    [Required(ErrorMessage = "User Name is required")]
    public string? Username { get; set; }
}

public class UserDto
{
    public string? Username { get; set; }
    public string Email { get; set; }
}
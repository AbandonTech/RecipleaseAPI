using System.ComponentModel.DataAnnotations;

namespace Reciplease;

public class User
{
    public int Id { get; set; }
    [Required] public string Username { get; set; }

    // Scope has default value of "User" defined in RecipleaseContext.cs
    [Required] public string Scope { get; set; }
}
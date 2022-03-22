using System.ComponentModel.DataAnnotations;

namespace Reciplease;

public class Ingredient
{
    [Key]
    [Required]
    public string Name { get; set; }
}
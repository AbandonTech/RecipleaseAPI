using System.ComponentModel.DataAnnotations;

namespace Reciplease;

public class Recipe
{
    public int Id { get; set; }
    public int Servings { get; set; }

    [Required] public string Name { get; set; }
}

public class CreateRecipeDto
{
    public int Servings { get; set; }

    public string Name { get; set; } = string.Empty;
}
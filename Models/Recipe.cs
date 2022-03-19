using System.ComponentModel.DataAnnotations;

namespace Reciplease;

public class Recipe 
{
    public int Id { get; set; }
    public int Servings { get; set; }

    [Required]
    public string Name { get; set; }
}

public class Ingredient
{
    [Key]
    [Required]
    public string Name { get; set; }
}

public class RecipeIngredient
{
    [Key]
    public int RecipeId { get; set; }
    public int IngredientId { get; set; }
    public int Quantity { get; set;}
    
    [Required]
    public string QuantityUnit { get; set; }
}
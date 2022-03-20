using System.ComponentModel.DataAnnotations;

namespace Reciplease;

public class RecipeIngredient
{
    [Key]
    public int RecipeId { get; set; }
    public int IngredientId { get; set; }
    public int Quantity { get; set;}
    
    [Required]
    public string QuantityUnit { get; set; }
}
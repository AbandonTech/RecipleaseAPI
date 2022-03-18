using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace Reciplease;

public class RecipeContext : DbContext 
{
    public DbSet<Recipe> Recipes { get; set; }

    public string DbPath { get; }

    public RecipeContext() 
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "reciplease.db");     
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}

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
    public int recipeId { get; set; }
    public int IngredientId { get; set; }
    public int Quantity { get; set;}
    
    [Required]
    public string QuantityUnit { get; set; }
}

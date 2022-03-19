using Microsoft.EntityFrameworkCore;
namespace Reciplease;

public class RecipleaseContext : DbContext 
{
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<RecipeIngredient> RecipeIngredients { get; set; }

    public string DbPath { get; }

    public RecipleaseContext() 
    {
        var CurrentDirectory = Environment.CurrentDirectory;
        var path = System.IO.Path.Join(CurrentDirectory, "TempData");
        
        System.IO.Directory.CreateDirectory(path);

        DbPath = System.IO.Path.Join(path, "reciplease.db");     
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Reciplease;

public class RecipleaseContext : IdentityDbContext<IdentityUser> 
{
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<RecipeIngredient> RecipeIngredients { get; set; }

    // These values are defined here as well as in the Program.cs, it is unclear to me how to use the values configured
    // In the Program.cs, but these should be removed in favor of the forced configuration.
    public string? googleClientId = Environment.GetEnvironmentVariable("GOOGLE_OAUTH_CLIENT_ID");
    public string? googleClientSecret = Environment.GetEnvironmentVariable("GOOGLE_OAUTH_CLIENT_SECRET");

    public string? JwtValidAudience = Environment.GetEnvironmentVariable("ValidAudience");
    public string? JwtValidIssuer = Environment.GetEnvironmentVariable("ValidIssuer");
    public string? JwtSecret = Environment.GetEnvironmentVariable("Secret");

    public string DbPath { get; }

    public RecipleaseContext(DbContextOptions<RecipleaseContext> options) : base(options)
    {
        var CurrentDirectory = Environment.CurrentDirectory;
        var path = System.IO.Path.Join(CurrentDirectory, "TempData");
        
        System.IO.Directory.CreateDirectory(path);

        DbPath = System.IO.Path.Join(path, "reciplease.db");     
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}
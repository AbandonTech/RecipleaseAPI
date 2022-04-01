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

    public string? googleClientId = Environment.GetEnvironmentVariable("GOOGLE_OAUTH_CLIENT_ID");
    public string? googleClientSecret = Environment.GetEnvironmentVariable("GOOGLE_OAUTH_CLIENT_SECRET");

    public string? JwtValidAudience = Environment.GetEnvironmentVariable("ValidAudience");
    public string? JwtValidIssuer = Environment.GetEnvironmentVariable("ValidIssuer");
    public string? JwtSecret = Environment.GetEnvironmentVariable("Secret");

    public string DbPath { get; }

    public RecipleaseContext(DbContextOptions<RecipleaseContext> options) : base(options)
    {
        if (googleClientId == null)
        {
            throw new ArgumentNullException(paramName: nameof(googleClientId), message: "Google OAuth Client Id cannot be null.");
        }
        if (googleClientSecret == null)
        {
            throw new ArgumentNullException(paramName: nameof(googleClientSecret), message: "Google OAuth Client Secret cannot be null.");
        }
        
        if (JwtValidAudience == null)
        {
            throw new ArgumentNullException(paramName: nameof(JwtValidAudience),
                message: "JWT ValidAudience cannot be null.");
        }
        
        if (JwtValidIssuer == null)
        {
            throw new ArgumentNullException(paramName: nameof(JwtValidIssuer),
                message: "JWT ValidIssuer cannot be null.");
        }
        
        if (JwtSecret == null)
        {
            throw new ArgumentNullException(paramName: nameof(JwtSecret), message: "Jwt Secret cannot be null.");
        }

        var CurrentDirectory = Environment.CurrentDirectory;
        var path = System.IO.Path.Join(CurrentDirectory, "TempData");
        
        System.IO.Directory.CreateDirectory(path);

        DbPath = System.IO.Path.Join(path, "reciplease.db");     
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}
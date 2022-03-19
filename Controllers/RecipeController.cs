using Microsoft.AspNetCore.Mvc;

namespace Reciplease.Controllers;

[ApiController]
[Route("[controller]")]
public class RecipeController : ControllerBase
{

    private readonly ILogger<RecipeController> _logger;
    private readonly RecipleaseContext _context;

    public RecipeController(ILogger<RecipeController> logger, RecipleaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet(Name = "GetRecipes")]
    public Recipe[] GetRecipes()
    {
        return _context.Recipes.ToArray();
    }

}
using Microsoft.AspNetCore.Mvc;

namespace Reciplease.Controllers;

[ApiController]
[Route("[controller]")]
public class RecipeController : ControllerBase
{

    private readonly ILogger<RecipeController> _logger;

    public RecipeController(ILogger<RecipeController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetRecipes")]
    public string GetRecipes()
    {
        return "test";
    }

}
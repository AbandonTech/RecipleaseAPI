using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Reciplease.Controllers;

[ApiController, Route("api/[controller]")]
public class RecipeController : ControllerBase
{
    private readonly ILogger<RecipeController> _logger;
    private readonly RecipleaseContext _context;

    public RecipeController(ILogger<RecipeController> logger, RecipleaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public ActionResult<Recipe[]> GetAllRecipes()
    {
        return _context.Recipes.ToArray();
    }

    [HttpPost]
    public ActionResult<Recipe> CreateRecipe(CreateRecipeDto recipe)
    {
        var newRecipe = new Recipe
        {
            Servings = recipe.Servings,
            Name = recipe.Name
        };

        try
        {
            newRecipe = _context.Recipes.Add(newRecipe).Entity;
            _context.SaveChanges();
            return newRecipe;
        }
        catch (DbUpdateException e)
        {
            _logger.LogError(e, "Unable to add new recipe: {Recipe}", recipe.ToString());
            return StatusCode(500);
        }
    }

    [HttpGet, Route("{recipeId:int}")]
    public ActionResult<Recipe> GetRecipe(int recipeId)
    {
        var recipe = _context.Recipes.Find(recipeId);

        if (recipe == null)
            return NotFound("a recipe with that id could not be found");

        return recipe;
    }

    [HttpDelete, Route("{recipeId:int}")]
    public ActionResult DeleteRecipe(int recipeId)
    {
        var recipeResult = GetRecipe(recipeId);

        if (recipeResult.Value == null) return recipeResult.Result!;

        _context.Recipes.Remove(recipeResult.Value!);
        _context.SaveChanges();
        return Ok();
    }

    [HttpPatch, Route("{recipeId:int}")]
    public ActionResult<Recipe> UpdateRecipe(int recipeId, CreateRecipeDto recipe)
    {
        var recipeResult = GetRecipe(recipeId);

        if (recipeResult.Value == null) return recipeResult.Result!;

        var recipeToUpdate = recipeResult.Value!;

        recipeToUpdate.Name = recipe.Name;
        recipeToUpdate.Servings = recipe.Servings;

        _context.Update(recipeToUpdate);
        _context.SaveChanges();
        return recipeToUpdate;
    }

    [HttpPut]
    public ActionResult<Recipe> UpdateOrCreateRecipe(Recipe recipe)
    {
        var oldRecipe = GetRecipe(recipe.Id).Value;
        
        if (oldRecipe == null)
            return CreateRecipe(new CreateRecipeDto {Name = recipe.Name, Servings = recipe.Servings});

        // Entity framework doesn't like us querying before hand, as it counts as a tracked entity.
        // Due to this we have to clear the tracker before updating the entity of the same id.
        _context.ChangeTracker.Clear();
        recipe = _context.Recipes.Update(recipe).Entity;
        _context.SaveChanges();
        return recipe;
    }
}
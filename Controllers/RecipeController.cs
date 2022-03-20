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

    /// <summary>
    /// Get all recipes
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public ActionResult<Recipe[]> GetAllRecipes()
    {
        return _context.Recipes.ToArray();
    }

    /// <summary>
    /// Create a new recipe
    /// </summary>
    /// <remarks>
    /// Creates a new recipe from the data provided, a new id will be assigned to this new recipe. <br/>
    /// The new recipe will then be returned.
    /// </remarks>
    /// <param name="recipe">New recipe data</param>
    /// <returns></returns>
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

    /// <summary>
    /// Get a recipe
    /// </summary>
    /// <remarks>
    /// Get a recipe using its id. <br />
    /// If a recipe with that id does not exists, a 404 response will be returned.
    /// </remarks>
    /// <param name="recipeId"></param>
    /// <returns></returns>
    [HttpGet, Route("{recipeId:int}")]
    public ActionResult<Recipe> GetRecipe(int recipeId)
    {
        var recipe = _context.Recipes.Find(recipeId);

        if (recipe == null)
            return NotFound("a recipe with that id could not be found");

        return recipe;
    }

    /// <summary>
    /// Delete a recipe
    /// </summary>
    /// <remarks>
    /// Delete a recipe using its id. <br />
    /// If a recipe with that id does not exists, a 404 response will be returned.
    /// </remarks>
    /// <param name="recipeId"></param>
    /// <returns></returns>
    [HttpDelete, Route("{recipeId:int}")]
    public ActionResult DeleteRecipe(int recipeId)
    {
        var recipeResult = GetRecipe(recipeId);

        if (recipeResult.Value == null) return recipeResult.Result!;

        _context.Recipes.Remove(recipeResult.Value!);
        _context.SaveChanges();
        return Ok();
    }

    /// <summary>
    /// Update a recipe
    /// </summary>
    /// <remarks>
    /// Update an existing recipe using its id. <br/>
    /// If a recipe with that id does not exists, a 404 response will be returned.
    /// </remarks>
    /// <param name="recipeId"></param>
    /// <param name="recipe"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Add or create a recipe
    /// </summary>
    /// <remarks>
    /// Updates an existing recipe with the provided data. If it doesn't exist, a new entry will be created. <br/>
    /// If a new entry is created, the returned recipe data will hold the new id.
    /// </remarks>
    /// <param name="recipe"></param>
    /// <returns></returns>
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
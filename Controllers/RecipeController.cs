using Microsoft.AspNetCore.Authorization;
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
    /// <response code="200">All recipes</response>
    [HttpGet, Route("all")]
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
    ///
    /// Validation:
    ///
    ///     * Servings must be greater than 0
    ///     * Name cannot be an empty string
    /// </remarks>
    /// <param name="recipe">New recipe data</param>
    /// <response code="201">The created recipe</response>
    /// <response code="400">Invalid data in request</response>
    [Authorize]
    [HttpPost]
    public ActionResult<Recipe> CreateRecipe(CreateRecipeDto recipe)
    {
        if (recipe.Servings <= 0) return BadRequest("Servings must be greater than 0");
        if (recipe.Name.Length == 0) return BadRequest("Recipe name cannot be empty");

        var newRecipe = new Recipe
        {
            Servings = recipe.Servings,
            Name = recipe.Name
        };

        try
        {
            newRecipe = _context.Recipes.Add(newRecipe).Entity;
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetRecipe), new {recipeId = newRecipe.Id}, newRecipe);
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
    /// <param name="recipeId">The id of the recipe to get</param>
    /// <response code="200">The recipe searched for</response>
    /// <response code="404">Recipe does not exist</response>
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
    /// <param name="recipeId">The id of the recipe to delete</param>
    /// <response code="200">Recipe successfully delete</response>
    /// <response code="404">Recipe does not exist</response>
    [Authorize]
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
    /// <response code="200">The recipe that was created</response>
    /// <response code="404">Recipe does not exist</response>
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
    /// <response code="200">The recipe that was updated</response>
    /// <response code="201">The recipe that was created</response>
    /// <response code="400">Invalid data in request</response>
    [HttpPut]
    public ActionResult<Recipe> UpdateOrCreateRecipe(Recipe recipe)
    {
        var oldRecipe = GetRecipe(recipe.Id).Value;

        if (oldRecipe == null)
        {
            var newRecipeDto = new CreateRecipeDto
            {
                Name = recipe.Name,
                Servings = recipe.Servings
            };

            return CreateRecipe(newRecipeDto);
        }


        // Entity framework doesn't like us querying before hand, as it counts as a tracked entity.
        // Due to this we have to clear the tracker before updating the entity of the same id.
        _context.ChangeTracker.Clear();
        recipe = _context.Recipes.Update(recipe).Entity;
        _context.SaveChanges();
        return recipe;
    }
}
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Exceptions;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Recipes.ValueObjects;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Recipes;

public static class GetRecipeById
{
    public record struct Request(Guid Id) : IRequestId<Request>;

    public sealed record RecipeIngredientDto(Guid IngredientId, string IngredientName, string? IngredientRecipeUrl, Guid UnitId, string UnitName, double Amount);

    public sealed record RecipeAmountDto(double Amount, Guid UnitId, string UnitName);

    public sealed record RecipeSectionDto(int SectionType, IEnumerable<RecipeStepDto> Steps);

    public sealed record RecipeStepDto(int Order, string Description, string? ImageUrl);

    public sealed record RecipeCategoryDto(Guid Id, string Name);

    public sealed record Response(string Title, string Description, string? ImageUrl, string? VideoUrl, RecipeAmountDto Amount, byte NumberOfServings, int Difficulty,
                                  IEnumerable<RecipeIngredientDto> Ingredients, IEnumerable<RecipeSectionDto> Sections, IEnumerable<RecipeCategoryDto> CategoryIds, Guid? IngredientId);

    [GroupEndpoint("Recipes")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardGet<Response>("{id}", Handler)
                     .WithName("GetRecipeById")
                     .WithDescription("Get a recibe by id");
        }
    }

    public static async Task<Results<Ok<Response>, BadRequest>> Handler(Request id, IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        Recipe? recipe = await GetRecipe(new RecipeId(id.Id), dbContext, cancellationToken);

        IEnumerable<Ingredient> recipeIngredients = await GetRecipeIngredients(recipe, dbContext, cancellationToken);
        IEnumerable<Unit> units = await dbContext.Units.AsNoTracking().ToListAsync(cancellationToken);

        Response response = MapToResponse(recipe, recipeIngredients, units);

        return TypedResults.Ok(response);
    }

    private static async Task<Recipe?> GetRecipe(RecipeId recipeId, IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        return await dbContext.Recipes.AsNoTracking()
                                      .Include(r => r.Ingredients)
                                      .Include(r => r.Sections)
                                      .Include(r => r.Categories)
                                      .AsSplitQuery()
                                      .FirstOrDefaultAsync(recipe => recipe.Id == recipeId, cancellationToken)

               ?? throw new EntityNotFoundException<Recipe, RecipeId>(recipeId);
    }

    private static async Task<IEnumerable<Ingredient>> GetRecipeIngredients(Recipe recipe, IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        IEnumerable<IngredientId> ingredientIds = recipe.GetIngredientIds();

        return await dbContext.Ingredients.AsNoTracking()
                                          .Where(i => ingredientIds.Contains(i.Id))
                                          .ToListAsync(cancellationToken);
    }

    private static Response MapToResponse(Recipe recipe, IEnumerable<Ingredient> recipeIngredients, IEnumerable<Unit> units)
    {
        IEnumerable<RecipeIngredientDto> ingredients = recipe.Ingredients.Select(ingredient => MapToIngredientDto(ingredient, recipeIngredients, units));
        IEnumerable<RecipeSectionDto> sections = recipe.Sections.Select(MapRecipeSectionDto);
        IEnumerable<RecipeCategoryDto> categories = recipe.Categories.Select(c => new RecipeCategoryDto(c.Id.Value, c.Name));

        return new Response
        (
            recipe.Title,
            recipe.Description ?? string.Empty,
            recipe.ImageURL,
            recipe.VideoURL,
            MapToRecipeAmountDto(recipe.Amount),
            recipe.NumberOfServings,
            (int)recipe.Difficulty,
            ingredients,
            sections,
            categories,
            recipe.IngredientId?.Value
        );
    }

    private static RecipeIngredientDto MapToIngredientDto(RecipeIngredient recipeIngredient, IEnumerable<Ingredient> ingredients, IEnumerable<Unit> units)
    {
        Ingredient? ingredient = ingredients.FirstOrDefault(i => i.Id == recipeIngredient.IngredientId);
        Unit? unit = units.FirstOrDefault(u => u.Id == recipeIngredient.UnitId);

        return new RecipeIngredientDto
        (
            recipeIngredient.IngredientId.Value,
            ingredient?.Name ?? string.Empty,
            ingredient?.Recipes.Any() == true ? ingredient.Recipes.First().Value.ToString() : null,
            recipeIngredient.UnitId.Value,
            unit?.ShortName != null ? unit.ShortName : unit?.Name ?? string.Empty,
            recipeIngredient.Amount
        );
    }

    private static RecipeAmountDto MapToRecipeAmountDto(RecipeAmount recipeAmount)
    {
        return new RecipeAmountDto
        (
            recipeAmount.Amount,
            recipeAmount.UnitId.Value,
            recipeAmount.Unit.Name
        );
    }

    private static RecipeSectionDto MapRecipeSectionDto(RecipeSection recipeSection)
    {
        return new RecipeSectionDto
        (
            (int)recipeSection.Type,
            recipeSection.Steps.Select(step => new RecipeStepDto(step.Order, step.Description, step.ImageUrl))
        );
    }
}

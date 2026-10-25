using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Exceptions;
using RecipeManager.Api.Domain.Common;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Recipes.Enums;
using RecipeManager.Api.Domain.Recipes.ValueObjects;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Api.Shared.Hateoas.Builder;
using RecipeManager.Api.Shared.Hateoas.Common;
using RecipeManager.Api.Shared.Hateoas.Models;
using LinkOptions = RecipeManager.Api.Shared.Hateoas.Models.LinkOptions;

namespace RecipeManager.Api.Features.Recipes;

public static class GetRecipeById
{
    private sealed record UnitInfo(UnitId Id, string Name, string? ShortName);

    public record struct Request(Guid Id) : IRequestId<Request>;

    public sealed record RecipeIngredientDto(IngredientDto Ingredient, UnitDto Unit, double Amount);

    public sealed record UnitDto(Guid Id, string Name);

    public sealed record RecipeAmountDto(double Amount, UnitDto Unit);

    public sealed record RecipeSectionDto(int SectionType, IEnumerable<RecipeStepDto> Steps, bool IsRequired);

    public sealed record RecipeStepDto(int Order, string Description, string? ImageUrl);

    public sealed record RecipeCategoryDto(Guid Id, string Name);

    public sealed record IngredientDto(Guid Id, string Name, string? IngredientRecipe);

    public sealed record NutritionalValueDto(int Calories, double Proteins, double Fats, double Carbohydrates, int IngredientAmount, Guid IngredientUnitId);

    public sealed record Response(Guid Id, string Title, string Description, string? ImageUrl, string? VideoUrl, RecipeAmountDto Amount, byte NumberOfServings, int Difficulty, NutritionalValueDto NutritionalValues,
                                  IEnumerable<RecipeIngredientDto> Ingredients, IEnumerable<RecipeSectionDto> Sections, IEnumerable<RecipeCategoryDto> Categories, IngredientDto? Ingredient);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardGet<Response>(RecipesGroup.RecipeByIdRoute, Handler)
                     .WithName("GetRecipeById")
                     .WithDescription("Get a recibe by id")
                     .WithTags(RecipesGroup.Tag)
                     .RequireAuthorization("RecipeManagerPolicy")
                     .ProducesProblem(StatusCodes.Status401Unauthorized);
        }
    }

    public static async Task<Results<Ok<HateoasResponse<Response>>, BadRequest>> Handler(Request id, [FromServices] ICurrentUser currentUser, [FromServices] IAppDbContext dbContext, [FromServices] IHateoasBuilderFactory hateoasBuilderFactory, CancellationToken cancellationToken)
    {
        Recipe? recipe = await GetRecipe(new RecipeId(id.Id), dbContext, cancellationToken);

        IEnumerable<Ingredient> recipeIngredients = await GetRecipeIngredients(recipe!, dbContext, cancellationToken);
        List<UnitInfo> units = await dbContext.Units.AsNoTracking()
            .Select(unit => new UnitInfo(unit.Id, unit.Name, unit.ShortName))
            .ToListAsync(cancellationToken);
        IEnumerable<RecipeCategory> categories = recipe!.Categories.Count > 0
                                               ? await dbContext.RecipeCategories.Where(category => recipe.Categories.Contains(category.Id))
                                                                                 .AsNoTracking()
                                                                                 .ToListAsync(cancellationToken)
                                               : [];

        Response response = MapToResponse(recipe, recipeIngredients, units, categories);
        HateoasResponseBuilder<Response> responseBuilder = hateoasBuilderFactory.ForItem(response);

        bool isRecipeCreator = recipe.CreatedBy == currentUser.Id;

        responseBuilder.AddDelete(LinkOptions.Create("DeleteRecipe", HateoasRelConstants.Delete, isRecipeCreator), new { RecipeId = recipe.Id })
                       .AddPut(LinkOptions.Create("UpdateRecipe", HateoasRelConstants.Update, isRecipeCreator), new { RecipeId = recipe.Id });

        return TypedResults.Ok(responseBuilder.Build());
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

    private static Response MapToResponse(Recipe recipe, IEnumerable<Ingredient> recipeIngredients, IEnumerable<UnitInfo> units, IEnumerable<RecipeCategory> recipeCategories)
    {
        IEnumerable<RecipeIngredientDto> ingredients = recipe.Ingredients.Select(ingredient => MapToIngredientDto(ingredient, recipeIngredients, units));
        IEnumerable<RecipeSectionDto> sections = recipe.Sections.Select(MapRecipeSectionDto);
        IEnumerable<RecipeCategoryDto> categories = recipeCategories.Select(c => new RecipeCategoryDto(c.Id.Value, c.Name));

        Ingredient? ingredient = recipeIngredients.FirstOrDefault(i => i.Id == recipe.IngredientId);
        IngredientDto? ingredientDto = ingredient == null ? null : new IngredientDto(ingredient.Id.Value, ingredient.Name, null);

        return new Response
        (
            recipe.Id.Value,
            recipe.Title,
            recipe.Description ?? string.Empty,
            recipe.ImageURL,
            recipe.VideoURL,
            MapToRecipeAmountDto(recipe.Amount, units),
            recipe.NumberOfServings,
            (int)recipe.Difficulty,
            MapToMapToNutritionalValuesDto(recipe.NutritionalValue),
            ingredients,
            sections,
            categories,
            ingredientDto
        );
    }

    private static RecipeIngredientDto MapToIngredientDto(RecipeIngredient recipeIngredient, IEnumerable<Ingredient> ingredients, IEnumerable<UnitInfo> units)
    {
        Ingredient? ingredient = ingredients.FirstOrDefault(i => i.Id == recipeIngredient.IngredientId);
        UnitInfo? unit = units.FirstOrDefault(u => u.Id == recipeIngredient.UnitId);

        return new RecipeIngredientDto
        (
            new IngredientDto
            (
                recipeIngredient.IngredientId.Value,
                ingredient?.Name ?? string.Empty,
                ingredient?.Recipes.Any() == true ? ingredient.Recipes[0].Value.ToString() : null
            ),
            new UnitDto
            (
                recipeIngredient.UnitId.Value,
                unit?.ShortName != null ? unit.ShortName : unit?.Name ?? string.Empty
            ),
            recipeIngredient.Amount
        );
    }

    private static RecipeAmountDto MapToRecipeAmountDto(RecipeAmount recipeAmount, IEnumerable<UnitInfo> units)
    {
        return new RecipeAmountDto
        (
            recipeAmount.Amount,
            new UnitDto
            (
                recipeAmount.UnitId.Value,
                units.First(unit => unit.Id == recipeAmount.UnitId)?.ShortName ?? string.Empty
            )
        );
    }

    private static RecipeSectionDto MapRecipeSectionDto(RecipeSection recipeSection)
    {
        return new RecipeSectionDto
        (
            (int)recipeSection.Type,
            recipeSection.Steps.Select(step => new RecipeStepDto(step.Order, step.Description, step.ImageUrl)),
            recipeSection.Type is RecipeSectionType.Cooking or RecipeSectionType.IngredientsPreparation
        );
    }

    private static NutritionalValueDto MapToMapToNutritionalValuesDto(NutritionalValue nutritionalValue)
    {
        return new
        (
            nutritionalValue.Calories,
            nutritionalValue.Proteins,
            nutritionalValue.Fats,
            nutritionalValue.Carbohydrates,
            nutritionalValue.IngredientAmount,
            nutritionalValue.IngredientUnit
        );
    }
}

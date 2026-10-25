using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Common;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Recipes.Enums;
using RecipeManager.Api.Domain.Recipes.ValueObjects;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Hateoas.Builder;
using RecipeManager.Api.Shared.Hateoas.Common;
using RecipeManager.Api.Shared.Hateoas.Models;
using LinkOptions = RecipeManager.Api.Shared.Hateoas.Models.LinkOptions;

namespace RecipeManager.Api.Features.Recipes.Query.GetRecipeById;

public sealed record GetRecipeByIdQuery(Guid RecipeId);
internal sealed record GetRecipeByIdProjection(
    Recipe Recipe,
    IEnumerable<Ingredient> Ingredients,
    IReadOnlyCollection<GetRecipeByIdUnitProjection> Units,
    IEnumerable<RecipeCategory> Categories);
internal sealed record GetRecipeByIdUnitProjection(UnitId Id, string Name, string? ShortName);
public sealed record GetRecipeByIdUnitDto(Guid Id, string Name);
public sealed record GetRecipeByIdAmountDto(double Amount, GetRecipeByIdUnitDto Unit);
public sealed record GetRecipeByIdStepDto(int Order, string Description, string? ImageUrl);
public sealed record GetRecipeByIdSectionDto(int SectionType, IEnumerable<GetRecipeByIdStepDto> Steps, bool IsRequired);
public sealed record GetRecipeByIdIngredientDto(Guid Id, string Name, string? IngredientRecipe);
public sealed record GetRecipeByIdRecipeIngredientDto(GetRecipeByIdIngredientDto Ingredient, GetRecipeByIdUnitDto Unit, double Amount);
public sealed record GetRecipeByIdCategoryDto(Guid Id, string Name);
public sealed record GetRecipeByIdNutritionalValueDto(
    int Calories, double Proteins, double Fats, double Carbohydrates, int IngredientAmount, Guid IngredientUnitId);
public sealed record GetRecipeByIdResponse(
    Guid Id,
    string Title,
    string Description,
    string? ImageUrl,
    string? VideoUrl,
    GetRecipeByIdAmountDto Amount,
    byte NumberOfServings,
    int Difficulty,
    GetRecipeByIdNutritionalValueDto NutritionalValues,
    IEnumerable<GetRecipeByIdRecipeIngredientDto> Ingredients,
    IEnumerable<GetRecipeByIdSectionDto> Sections,
    IEnumerable<GetRecipeByIdCategoryDto> Categories,
    GetRecipeByIdIngredientDto? Ingredient);

public sealed class GetRecipeByIdQueryHandler(
    IAppDbContext dbContext,
    ICurrentUser currentUser,
    IHateoasBuilderFactory hateoasBuilderFactory)
{
    public async Task<Result<HateoasResponse<GetRecipeByIdResponse>>> Handle(
        GetRecipeByIdQuery query,
        CancellationToken cancellationToken)
    {
        RecipeId id = new(query.RecipeId);
        Recipe? recipe = await dbContext.Recipes.AsNoTracking()
            .Include(item => item.Ingredients)
            .Include(item => item.Sections)
            .Include(item => item.Categories)
            .AsSplitQuery()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (recipe is null)
        {
            return new NotFoundError();
        }

        IEnumerable<IngredientId> ingredientIds = recipe.GetIngredientIds();
        List<Ingredient> ingredients = await dbContext.Ingredients.AsNoTracking()
            .Where(ingredient => ingredientIds.Contains(ingredient.Id))
            .ToListAsync(cancellationToken);
        List<GetRecipeByIdUnitProjection> units = await dbContext.Units.AsNoTracking()
            .Select(unit => new GetRecipeByIdUnitProjection(unit.Id, unit.Name, unit.ShortName))
            .ToListAsync(cancellationToken);
        IEnumerable<RecipeCategory> categories = recipe.Categories.Count > 0
            ? await dbContext.RecipeCategories.Where(category => recipe.Categories.Contains(category.Id))
                .AsNoTracking().ToListAsync(cancellationToken)
            : [];

        GetRecipeByIdProjection projection = new(recipe, ingredients, units, categories);
        GetRecipeByIdResponse response = MapToResponse(projection);
        HateoasResponseBuilder<GetRecipeByIdResponse> responseBuilder = hateoasBuilderFactory.ForItem(response);
        bool isRecipeCreator = recipe.CreatedBy == currentUser.Id;
        responseBuilder
            .AddDelete(LinkOptions.Create("DeleteRecipe", HateoasRelConstants.Delete, isRecipeCreator),
                new { RecipeId = recipe.Id })
            .AddPut(LinkOptions.Create("UpdateRecipe", HateoasRelConstants.Update, isRecipeCreator),
                new { RecipeId = recipe.Id });
        return responseBuilder.Build();
    }

    private static GetRecipeByIdResponse MapToResponse(GetRecipeByIdProjection projection)
    {
        Recipe recipe = projection.Recipe;
        var ingredients = recipe.Ingredients.Select(item =>
            MapIngredient(item, projection.Ingredients, projection.Units));
        var sections = recipe.Sections.Select(section => new GetRecipeByIdSectionDto(
            (int)section.Type,
            section.Steps.Select(step => new GetRecipeByIdStepDto(step.Order, step.Description, step.ImageUrl)),
            section.Type is RecipeSectionType.Cooking or RecipeSectionType.IngredientsPreparation));
        var categories = projection.Categories.Select(category =>
            new GetRecipeByIdCategoryDto(category.Id.Value, category.Name));

        Ingredient? recipeIngredient = projection.Ingredients.FirstOrDefault(item => item.Id == recipe.IngredientId);
        GetRecipeByIdIngredientDto? recipeIngredientDto = recipeIngredient is null
            ? null
            : new GetRecipeByIdIngredientDto(
                recipeIngredient.Id.Value,
                recipeIngredient.Name,
                null);
        return new GetRecipeByIdResponse(
            recipe.Id.Value,
            recipe.Title,
            recipe.Description ?? string.Empty,
            recipe.ImageURL,
            recipe.VideoURL,
            new GetRecipeByIdAmountDto(
                recipe.Amount.Amount,
                new GetRecipeByIdUnitDto(
                    recipe.Amount.UnitId.Value,
                    projection.Units.First(unit => unit.Id == recipe.Amount.UnitId).ShortName ?? string.Empty)),
            recipe.NumberOfServings,
            (int)recipe.Difficulty,
            new GetRecipeByIdNutritionalValueDto(
                recipe.NutritionalValue.Calories,
                recipe.NutritionalValue.Proteins,
                recipe.NutritionalValue.Fats,
                recipe.NutritionalValue.Carbohydrates,
                recipe.NutritionalValue.IngredientAmount,
                recipe.NutritionalValue.IngredientUnit),
            ingredients,
            sections,
            categories,
            recipeIngredientDto);
    }

    private static GetRecipeByIdRecipeIngredientDto MapIngredient(
        RecipeIngredient recipeIngredient,
        IEnumerable<Ingredient> ingredients,
        IEnumerable<GetRecipeByIdUnitProjection> units)
    {
        Ingredient? ingredient = ingredients.FirstOrDefault(item => item.Id == recipeIngredient.IngredientId);
        GetRecipeByIdUnitProjection? unit = units.FirstOrDefault(item => item.Id == recipeIngredient.UnitId);
        return new GetRecipeByIdRecipeIngredientDto(
            new GetRecipeByIdIngredientDto(
                recipeIngredient.IngredientId.Value,
                ingredient?.Name ?? string.Empty,
                ingredient?.Recipes.Any() == true ? ingredient.Recipes[0].Value.ToString() : null),
            new GetRecipeByIdUnitDto(
                recipeIngredient.UnitId.Value,
                unit?.ShortName != null ? unit.ShortName : unit?.Name ?? string.Empty),
            recipeIngredient.Amount);
    }
}

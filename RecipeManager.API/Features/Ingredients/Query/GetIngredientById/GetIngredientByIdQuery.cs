using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Common;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Features.Units;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.Ingredients.Query.GetIngredientById;

public sealed record GetIngredientByIdQuery(Guid IngredientId);

public sealed record GetIngredientByIdNutritionalValueDto(
    int Calories,
    double Proteins,
    double Fats,
    double Carbohydrates,
    int IngredientAmount,
    Guid IngredientUnitId);

public sealed record GetIngredientByIdRecipeDto(Guid Id, string Title, ItemActionDto Actions);

public sealed record GetIngredientByIdCategoryDto(Guid Id, string Name);

public sealed record GetIngredientByIdUnitConvertionDto(Guid UnitToConvertId, double Ratio);

public sealed record GetIngredientByIdPackageDto(Guid PackageUnitId, int PackageSize, Guid PackageSizeUnitId);

public sealed record GetIngredientByIdIngredientDto(
    Guid Id,
    string Name,
    GetIngredientByIdNutritionalValueDto NutritionalValues,
    Guid BaseUnit,
    GetIngredientByIdPackageDto IngredientPackage,
    IEnumerable<GetIngredientByIdUnitConvertionDto> IngredientUnitConvertions,
    GetIngredientByIdCategoryDto ShoppingListCategory,
    IEnumerable<GetIngredientByIdRecipeDto> Recipes,
    IEnumerable<GetIngredientByIdCategoryDto> Categories);

public sealed record GetIngredientByIdResponse(
    Guid Id,
    string Name,
    GetIngredientByIdNutritionalValueDto NutritionalValues,
    Guid BaseUnit,
    GetIngredientByIdPackageDto IngredientPackage,
    IEnumerable<GetIngredientByIdUnitConvertionDto> IngredientUnitConvertions,
    GetIngredientByIdCategoryDto ShoppingListCategory,
    IEnumerable<GetIngredientByIdRecipeDto> Recipes,
    IEnumerable<GetIngredientByIdCategoryDto> Categories,
    ItemActionDto Actions);

public sealed class GetIngredientByIdQueryHandler(
    IAppDbContext dbContext,
    ICurrentUser currentUser)
{
    public async Task<Result<GetIngredientByIdResponse>> Handle(
        GetIngredientByIdQuery query,
        CancellationToken cancellationToken)
    {
        IngredientId id = new(query.IngredientId);
        Ingredient? ingredient = await GetIngredient(id, dbContext, cancellationToken);
        if (ingredient is null)
        {
            return new NotFoundError();
        }

        IEnumerable<GetIngredientByIdRecipeDto> recipes =
            await GetIngredientRecipes(dbContext, ingredient, cancellationToken);
        IEnumerable<GetIngredientByIdCategoryDto> categories =
            await GetIngredientCategories(dbContext, ingredient, cancellationToken);

        GetIngredientByIdIngredientDto ingredientDto = MapToIngredientDto(
            ingredient,
            categories.FirstOrDefault(category => category.Id == ingredient.ShoppingListCategoryId?.Value),
            recipes,
            categories.Where(category => category.Id != ingredient.ShoppingListCategoryId?.Value));

        bool isIngredientCreator = ingredient.CreatedBy == currentUser.Id;
        return MapToResponse(ingredientDto, isIngredientCreator);
    }

    private static async Task<Ingredient?> GetIngredient(
        IngredientId ingredientId,
        IAppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return await dbContext.Ingredients.AsNoTracking()
            .Include(ingredient => ingredient.Categories)
            .Include(ingredient => ingredient.Recipes)
            .Include(ingredient => ingredient.IngredientPackage)
            .AsSplitQuery()
            .FirstOrDefaultAsync(ingredient => ingredient.Id == ingredientId, cancellationToken);
    }

    private static async Task<IEnumerable<GetIngredientByIdRecipeDto>> GetIngredientRecipes(
        IAppDbContext dbContext,
        Ingredient ingredient,
        CancellationToken cancellationToken)
    {
        return ingredient.Recipes.Any()
            ? await dbContext.Recipes.AsNoTracking()
                .Where(recipe => ingredient.Recipes.Contains(recipe.Id))
                .Select(recipe => new GetIngredientByIdRecipeDto(
                    recipe.Id,
                    recipe.Title,
                    new ItemActionDto(true, false, false)))
                .ToListAsync(cancellationToken)
            : [];
    }

    private static async Task<IEnumerable<GetIngredientByIdCategoryDto>> GetIngredientCategories(
        IAppDbContext dbContext,
        Ingredient ingredient,
        CancellationToken cancellationToken)
    {
        return ingredient.Categories.Any() || ingredient.ShoppingListCategoryId is not null
            ? await dbContext.IngredientCategories.AsNoTracking()
                .Where(category =>
                    ingredient.Categories.Contains(category.Id) ||
                    category.Id == ingredient.ShoppingListCategoryId)
                .Select(category => new GetIngredientByIdCategoryDto(category.Id, category.Name))
                .ToListAsync(cancellationToken)
            : [];
    }

    private static GetIngredientByIdIngredientDto MapToIngredientDto(
        Ingredient ingredient,
        GetIngredientByIdCategoryDto? shoppingListCategory,
        IEnumerable<GetIngredientByIdRecipeDto> recipes,
        IEnumerable<GetIngredientByIdCategoryDto> categories)
    {
        return new GetIngredientByIdIngredientDto(
            ingredient.Id.Value,
            ingredient.Name,
            MapToNutritionalValuesDto(ingredient.NutritionalValue),
            ingredient.BaseUnit!.Value,
            new GetIngredientByIdPackageDto(
                ingredient.IngredientPackage.PackageUnitId.Value,
                ingredient.IngredientPackage.PackageSize,
                ingredient.IngredientPackage.PackageSizeUnitId.Value),
            ingredient.IngredientUnitConvertions.Select(conversion =>
                new GetIngredientByIdUnitConvertionDto(conversion.UnitToConvertId.Value, conversion.Ratio)),
            shoppingListCategory!,
            recipes,
            categories);
    }

    private static GetIngredientByIdNutritionalValueDto MapToNutritionalValuesDto(NutritionalValue nutritionalValue)
    {
        return new GetIngredientByIdNutritionalValueDto(
            nutritionalValue.Calories,
            nutritionalValue.Proteins,
            nutritionalValue.Fats,
            nutritionalValue.Carbohydrates,
            nutritionalValue.IngredientAmount,
            nutritionalValue.IngredientUnit);
    }

    private static GetIngredientByIdResponse MapToResponse(
        GetIngredientByIdIngredientDto ingredientDto,
        bool isIngredientCreator)
    {
        return new GetIngredientByIdResponse(
            ingredientDto.Id,
            ingredientDto.Name,
            ingredientDto.NutritionalValues,
            ingredientDto.BaseUnit,
            ingredientDto.IngredientPackage,
            ingredientDto.IngredientUnitConvertions,
            ingredientDto.ShoppingListCategory,
            ingredientDto.Recipes,
            ingredientDto.Categories,
            new ItemActionDto(true, isIngredientCreator, isIngredientCreator));
    }
}

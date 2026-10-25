using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Common;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Hateoas.Builder;
using RecipeManager.Api.Shared.Hateoas.Common;
using RecipeManager.Api.Shared.Hateoas.Models;
using LinkOptions = RecipeManager.Api.Shared.Hateoas.Models.LinkOptions;

namespace RecipeManager.Api.Features.Ingredients.Query.GetIngredientById;

public sealed record GetIngredientByIdQuery(Guid IngredientId);

public sealed record GetIngredientByIdNutritionalValueDto(
    int Calories,
    double Proteins,
    double Fats,
    double Carbohydrates,
    int IngredientAmount,
    Guid IngredientUnitId);

public sealed record GetIngredientByIdRecipeDto(Guid Id, string Title);

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
    IEnumerable<HateoasResponse<GetIngredientByIdRecipeDto>> Recipes,
    IEnumerable<GetIngredientByIdCategoryDto> Categories);

public sealed class GetIngredientByIdQueryHandler(
    IAppDbContext dbContext,
    ICurrentUser currentUser,
    IHateoasBuilderFactory hateoasBuilderFactory)
{
    public async Task<Result<HateoasResponse<GetIngredientByIdResponse>>> Handle(
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
        return MapToHateoasResponse(ingredientDto, hateoasBuilderFactory, isIngredientCreator);
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
                .Select(recipe => new GetIngredientByIdRecipeDto(recipe.Id, recipe.Title))
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

    private static HateoasResponse<GetIngredientByIdResponse> MapToHateoasResponse(
        GetIngredientByIdIngredientDto ingredientDto,
        IHateoasBuilderFactory builderFactory,
        bool isIngredientCreator)
    {
        HateoasCollectionResponseBuilder<GetIngredientByIdRecipeDto> recipesBuilder =
            builderFactory.ForCollection(ingredientDto.Recipes);
        recipesBuilder.WithCollectionLink()
            .WithGet(
                LinkOptions.Create("GetRecipeById", HateoasRelConstants.Self, true),
                recipe => new { recipeId = recipe.Id });

        GetIngredientByIdResponse response = new(
            ingredientDto.Id,
            ingredientDto.Name,
            ingredientDto.NutritionalValues,
            ingredientDto.BaseUnit,
            ingredientDto.IngredientPackage,
            ingredientDto.IngredientUnitConvertions,
            ingredientDto.ShoppingListCategory,
            recipesBuilder.Build().Items,
            ingredientDto.Categories);

        HateoasResponseBuilder<GetIngredientByIdResponse> responseBuilder = builderFactory.ForItem(response);
        responseBuilder.AddGet(
                LinkOptions.Create("GetIngredientById", HateoasRelConstants.Self, true),
                new { ingredientId = ingredientDto.Id })
            .AddPut(
                LinkOptions.Create("UpdateIngredient", HateoasRelConstants.Update, isIngredientCreator),
                new { ingredientId = ingredientDto.Id })
            .AddDelete(
                LinkOptions.Create("DeleteIngredient", HateoasRelConstants.Delete, isIngredientCreator),
                new { ingredientId = ingredientDto.Id });

        return responseBuilder.Build();
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Persistance.Extensions;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Hateoas.Builder;
using RecipeManager.Api.Shared.Hateoas.Common;
using RecipeManager.Api.Shared.Hateoas.Models;
using LinkOptions = RecipeManager.Api.Shared.Hateoas.Models.LinkOptions;

namespace RecipeManager.Api.Features.Ingredients.Query.GetIngredients;

[BindProperties]
public sealed record GetIngredientsFilterParameters(string Category = "", int Page = 1, int PageSize = 10)
    : PagedParameters(Page, PageSize);

public sealed record GetIngredientsQuery(GetIngredientsFilterParameters Filter);

public sealed record GetIngredientsNutritionalValueDto(
    int Calories,
    double Proteins,
    double Fats,
    double Carbohydrates,
    int IngredientAmount,
    string IngredientUnit);

public sealed record GetIngredientsUnitConvertionDto(string UnitToConvert, double Ratio);

public sealed record GetIngredientsIngredientDto(
    Guid Id,
    string Name,
    string BaseUnit,
    IEnumerable<GetIngredientsUnitConvertionDto> IngredientUnitConvertions,
    GetIngredientsNutritionalValueDto NutritionalValues);

public sealed record GetIngredientsResponse(
    int Page,
    int PageSize,
    int TotalCount,
    IEnumerable<HateoasResponse<GetIngredientsIngredientDto>> Ingredients)
    : PagedResult(Page, PageSize, TotalCount);

public sealed class GetIngredientsQueryHandler(
    IAppDbContext dbContext,
    ICurrentUser currentUser,
    IHateoasBuilderFactory hateoasBuilderFactory)
{
    public async Task<Result<HateoasResponse<GetIngredientsResponse>>> Handle(
        GetIngredientsQuery query,
        CancellationToken cancellationToken)
    {
        GetIngredientsFilterParameters filter = query.Filter;
        IEnumerable<IngredientCategoryId> categoryIds = await GetFilteredCategories(
            filter.Category,
            dbContext,
            cancellationToken);
        Dictionary<UnitId, string> unitNames = await dbContext.Units.AsNoTracking()
            .Select(unit => new { unit.Id, unit.Name })
            .ToDictionaryAsync(unit => unit.Id, unit => unit.Name, cancellationToken);

        IQueryable<Ingredient> ingredientsQuery = PrepareIngredientsQuery(dbContext, categoryIds, currentUser.Id);
        int totalCount = await ingredientsQuery.CountAsync(cancellationToken);
        List<Ingredient> ingredients = await ingredientsQuery.SetPage(filter).ToListAsync(cancellationToken);

        List<GetIngredientsIngredientDto> ingredientDtos = ingredients.Select(ingredient =>
            new GetIngredientsIngredientDto(
                ingredient.Id.Value,
                ingredient.Name,
                ingredient.BaseUnit != null ? unitNames[ingredient.BaseUnit] : string.Empty,
                ingredient.IngredientUnitConvertions.Select(conversion =>
                    new GetIngredientsUnitConvertionDto(unitNames[conversion.UnitToConvertId], conversion.Ratio)),
                new GetIngredientsNutritionalValueDto(
                    ingredient.NutritionalValue.Calories,
                    ingredient.NutritionalValue.Proteins,
                    ingredient.NutritionalValue.Fats,
                    ingredient.NutritionalValue.Carbohydrates,
                    ingredient.NutritionalValue.IngredientAmount,
                    unitNames[ingredient.NutritionalValue.IngredientUnit])))
            .ToList();

        IEnumerable<HateoasResponse<GetIngredientsIngredientDto>> ingredientHateoas =
            MapIngredientsToHateoasResponse(ingredientDtos, hateoasBuilderFactory);
        HateoasResponse<GetIngredientsResponse> response = MapToHateoasResponse(
            filter,
            totalCount,
            ingredientHateoas,
            hateoasBuilderFactory);

        return response;
    }

    private static async Task<IEnumerable<IngredientCategoryId>> GetFilteredCategories(
        string category,
        IAppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return string.IsNullOrEmpty(category)
            ? []
            : await dbContext.IngredientCategories
                .Where(item => item.Name.ToLower().Contains(category.ToLower()))
                .Select(item => item.Id)
                .ToListAsync(cancellationToken);
    }

    private static IQueryable<Ingredient> PrepareIngredientsQuery(
        IAppDbContext dbContext,
        IEnumerable<IngredientCategoryId> categoryIds,
        string userId)
    {
        IQueryable<Ingredient> query = dbContext.Ingredients.AsNoTracking()
            .Where(ingredient => ingredient.CreatedBy == userId);

        if (categoryIds.Any())
        {
            query = query.Where(ingredient =>
                ingredient.Categories.Any(categoryId =>
                    categoryIds.Select(category => category.Value).Contains(categoryId.Value)));
        }

        return query.OrderBy(ingredient => ingredient.Name);
    }

    private static IEnumerable<HateoasResponse<GetIngredientsIngredientDto>> MapIngredientsToHateoasResponse(
        IEnumerable<GetIngredientsIngredientDto> ingredients,
        IHateoasBuilderFactory builderFactory)
    {
        if (!ingredients.Any())
        {
            return [];
        }

        HateoasCollectionResponseBuilder<GetIngredientsIngredientDto> ingredientsBuilder =
            builderFactory.ForCollection(ingredients);
        ingredientsBuilder.WithCollectionLink()
            .WithGet(
                LinkOptions.Create("GetIngredientById", HateoasRelConstants.Update, true),
                ingredient => new { ingredientId = ingredient.Id })
            .WithDelete(
                LinkOptions.Create("DeleteIngredient", HateoasRelConstants.Delete, true),
                ingredient => new { ingredientId = ingredient.Id });

        return ingredientsBuilder.Build().Items;
    }

    private static HateoasResponse<GetIngredientsResponse> MapToHateoasResponse(
        GetIngredientsFilterParameters filter,
        int totalCount,
        IEnumerable<HateoasResponse<GetIngredientsIngredientDto>> ingredients,
        IHateoasBuilderFactory builderFactory)
    {
        GetIngredientsResponse response = new(filter.Page, filter.PageSize, totalCount, ingredients);

        HateoasResponseBuilder<GetIngredientsResponse> responseBuilder = builderFactory.ForItem(response)
            .AddPagedNavigation("GetIngredients", new { filter.Category, filter.Page, filter.PageSize })
            .AddPost(LinkOptions.Create("CreateIngredient", HateoasRelConstants.Create, true), null);

        return responseBuilder.Build();
    }
}

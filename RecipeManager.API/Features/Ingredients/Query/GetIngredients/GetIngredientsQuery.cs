using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Features.Units;
using RecipeManager.Api.Persistance.Extensions;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Hateoas.Models;

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
    GetIngredientsNutritionalValueDto NutritionalValues,
    ItemActionDto Actions);

public sealed record GetIngredientsResponse(
    int Page,
    int PageSize,
    int TotalCount,
    CollectionActionDto Actions,
    IEnumerable<GetIngredientsIngredientDto> Ingredients)
    : PagedResult(Page, PageSize, TotalCount);

public sealed class GetIngredientsQueryHandler(
    IAppDbContext dbContext,
    ICurrentUser currentUser)
{
    public async Task<Result<GetIngredientsResponse>> Handle(
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
                    unitNames[ingredient.NutritionalValue.IngredientUnit]),
                new ItemActionDto(true, true, true)))
            .ToList();

        return new GetIngredientsResponse(
            filter.Page,
            filter.PageSize,
            totalCount,
            new CollectionActionDto(true),
            ingredientDtos);
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
}

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Persistance.Extensions;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Api.Shared.Hateoas.Builder;
using RecipeManager.Api.Shared.Hateoas.Common;
using RecipeManager.Api.Shared.Hateoas.Models;
using Scalar.AspNetCore;
using LinkOptions = RecipeManager.Api.Shared.Hateoas.Models.LinkOptions;

namespace RecipeManager.Api.Features.Ingredients;

public static class GetIngredients
{
    [BindProperties]
    public sealed record GetIngredientsFilterParameters(string Category = "", int Page = 1, int PageSize = 10) : PagedParameters(Page, PageSize);

    public sealed record NutritionalValueDto(int Calories, double Proteins, double Fats, double Carbohydrates, int IngredientAmount, Guid IngredientUnitId);

    public sealed record IngredientUnitConvertionDto(Guid UnitToConvertId, double Ratio);

    public sealed record IngredientDto(Guid Id, string Name, Guid BaseUnit, IEnumerable<IngredientUnitConvertionDto> IngredientUnitConvertions, NutritionalValueDto NutritionalValues);

    public sealed record Response(int Page, int PageSize, int TotalCount, IEnumerable<HateoasResponse<IngredientDto>> Ingredients) : PagedResult(Page, PageSize, TotalCount);

    [GroupEndpoint("Ingredients")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardGet<Response>("/", Handler)
                     .WithName("GetIngredients")
                     .WithDescription("Gets a paginated list of ingredients");
        }
    }

    public static async Task<Results<Ok<HateoasResponse<Response>>, BadRequest>> Handler([AsParameters] GetIngredientsFilterParameters filter, [FromServices] IHateoasBuilderFactory hateoasBuilderFactory, [FromServices] ICurrentUser currentUser, [FromServices] IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        IEnumerable<IngredientCategoryId> categoryIds = await GetFilteredCategories(filter.Category, dbContext, cancellationToken);

        IQueryable<Ingredient> query = PrepareIngredientsQuery(dbContext, categoryIds, currentUser.Id);

        int totalCount = await query.CountAsync(cancellationToken);

        List<Ingredient> ingredients = await query.SetPage(filter).ToListAsync(cancellationToken);
        List<IngredientDto> ingredientDtos = ingredients.Select(ingredient => new IngredientDto(
            ingredient.Id.Value,
            ingredient.Name,
            ingredient.BaseUnit.Value,
            ingredient.IngredientUnitConvertions.Select(c => new IngredientUnitConvertionDto(c.UnitToConvertId.Value, c.Ratio)),
            new NutritionalValueDto(
                ingredient.NutritionalValue.Calories,
                ingredient.NutritionalValue.Proteins,
                ingredient.NutritionalValue.Fats,
                ingredient.NutritionalValue.Carbohydrates,
                ingredient.NutritionalValue.IngredientAmount,
                ingredient.NutritionalValue.IngredientUnit.Value
            )
        )).ToList();

        IEnumerable<HateoasResponse<IngredientDto>> ingredientHateoas = MapIngredientsToHateoasResponse(ingredientDtos, hateoasBuilderFactory);
        HateoasResponse<Response> response = MapToHateoasResponse(filter, totalCount, ingredientHateoas, hateoasBuilderFactory);

        return TypedResults.Ok(response);
    }

    private static async Task<IEnumerable<IngredientCategoryId>> GetFilteredCategories(string category, IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        return string.IsNullOrEmpty(category)
             ? []
             : await dbContext.IngredientCategories.Where(c => c.Name.ToLower().Contains(category.ToLower()))
                                                   .Select(c => c.Id)
                                                   .ToListAsync(cancellationToken);
    }

    private static IQueryable<Ingredient> PrepareIngredientsQuery(IAppDbContext dbContext, IEnumerable<IngredientCategoryId> categoryIds, string userId)
    {
        IQueryable<Ingredient> query = dbContext.Ingredients.AsNoTracking().Where(ingredient => ingredient.CreatedBy == userId);

        if (categoryIds.Any())
        {
            query = query.Where(ingredient => ingredient.Categories.Any(categoryId => categoryIds.Select(c => c.Value).Contains(categoryId.Value)));
        }

        return query.OrderBy(ingredient => ingredient.Name);
    }

    private static IEnumerable<HateoasResponse<IngredientDto>> MapIngredientsToHateoasResponse(IEnumerable<IngredientDto> ingredients, IHateoasBuilderFactory hateoasBuilderFactory)
    {
        if (!ingredients.Any())
        {
            return [];
        }

        HateoasCollectionResponseBuilder<IngredientDto> ingredientsBuilder = hateoasBuilderFactory.ForCollection(ingredients);

        ingredientsBuilder.WithCollectionLink()
                            .WithGet(LinkOptions.Create("GetIngredientById", HateoasRelConstants.Update, true), ingredient => new { ingredientId = ingredient.Id })
                            .WithDelete(LinkOptions.Create("DeleteIngredient", HateoasRelConstants.Delete, true), ingredient => new { ingredientId = ingredient.Id });

        return ingredientsBuilder.Build().Items;
    }

    private static HateoasResponse<Response> MapToHateoasResponse(GetIngredientsFilterParameters filter, int totalCount, IEnumerable<HateoasResponse<IngredientDto>> ingredients, IHateoasBuilderFactory hateoasBuilderFactory)
    {
        Response response = new(filter.Page, filter.PageSize, totalCount, ingredients);

        HateoasResponseBuilder<Response> responseBuilder = hateoasBuilderFactory.ForItem(response)
                                                                                .AddPagedNavigation("GetIngredients", new { filter.Category, filter.Page, filter.PageSize })
                                                                                .AddPost(LinkOptions.Create("CreateIngredient", HateoasRelConstants.Create, true), null);
        return responseBuilder.Build();
    }
}

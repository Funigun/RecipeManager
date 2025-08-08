using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Persistance.Extensions;
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

    public sealed record IngredientDto(Guid Id, string Name);

    public sealed record Response(int Page, int PageSize, int TotalCount, IEnumerable<HateoasResponse<IngredientDto>> Ingredients) : PagedResult(Page, PageSize, TotalCount);

    [GroupEndpoint("Ingredients")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardAuthenticatedGet<Response>("/", Handler)
                     .WithName("GetIngredients")
                     .WithDescription("Gets a paginated list of ingredients")
                     .WithOpenApi(operation =>
                     {
                         operation.Description = "Filter query parameter allows to filter ingredients based on categoryname  (starts with)\nYou can also specify Page (default is 1) and Page Size (default is 50)";
                         return operation;
                     })
                     .CodeSample("A", ScalarTarget.CSharp);
        }
    }

    public static async Task<Results<Ok<HateoasResponse<Response>>, BadRequest>> Handler([AsParameters] GetIngredientsFilterParameters filter, IHateoasBuilderFactory hateoasBuilderFactory, IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        IEnumerable<IngredientCategoryId> categoryIds = await GetFilteredCategories(filter.Category, dbContext, cancellationToken);

        IQueryable<Ingredient> query = PrepareIngredientsQuery(dbContext, categoryIds);

        int totalCount = await query.CountAsync(cancellationToken);

        List<Ingredient> ingredients = await query.SetPage(filter).ToListAsync(cancellationToken);
        List<IngredientDto> ingredientDtos = ingredients.Select(ingredient => new IngredientDto(ingredient.Id.Value, ingredient.Name)).ToList();

        IEnumerable<HateoasResponse<IngredientDto>> ingredientHateoas = MapIngredientsToHateoasResponse(ingredientDtos, hateoasBuilderFactory);
        HateoasResponse<Response> response = MapToHateoasResponse(filter, totalCount, ingredientHateoas, hateoasBuilderFactory);

        return TypedResults.Ok(response);
    }

    private static async Task<IEnumerable<IngredientCategoryId>> GetFilteredCategories(string category, IAppDbContext dbContext, CancellationToken cancellationToken)
    {
        return string.IsNullOrEmpty(category)
             ? []
             : await dbContext.IngredientCategories.Where(c => c.Name.Contains(category, StringComparison.OrdinalIgnoreCase))
                                                   .Select(c => c.Id)
                                                   .ToListAsync(cancellationToken);
    }

    private static IQueryable<Ingredient> PrepareIngredientsQuery(IAppDbContext dbContext, IEnumerable<IngredientCategoryId> categoryIds)
    {
        IQueryable<Ingredient> query = dbContext.Ingredients.AsNoTracking();

        if (categoryIds.Any())
        {
            query = query.Where(ingredient => ingredient.Categories.Any(categoryId => categoryIds.Contains(categoryId)));
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
                          .WithGet(LinkOptions.Create("GetIngredientById", HateoasRelConstants.Self, true), ingredient => new { id = ingredient.Id });

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

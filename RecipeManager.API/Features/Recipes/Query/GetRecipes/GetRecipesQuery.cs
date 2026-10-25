using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Persistance.Extensions;
using RecipeManager.Api.Shared.Hateoas.Builder;
using RecipeManager.Api.Shared.Hateoas.Common;
using RecipeManager.Api.Shared.Hateoas.Models;
using LinkOptions = RecipeManager.Api.Shared.Hateoas.Models.LinkOptions;

namespace RecipeManager.Api.Features.Recipes.Query.GetRecipes;

public sealed class ItemIds
{
    public List<Guid> Ids = [];

    public static bool TryParse(string? value, IFormatProvider? provider, out ItemIds? itemIds)
    {
        string? trimmedValue = value?.TrimStart('(').TrimEnd(')');
        string[]? segments = trimmedValue?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (segments is null)
        {
            itemIds = new ItemIds();
            return false;
        }

        List<Guid> ids = [];
        foreach (string segment in segments)
        {
            if (Guid.TryParse(segment, out Guid id))
            {
                ids.Add(id);
            }
        }

        itemIds = new ItemIds { Ids = ids };
        return true;
    }
}

public sealed record GetRecipesParameters(ItemIds? Categories, ItemIds? Ingredients, int Page = 1, int PageSize = 10)
    : PagedParameters(Page, PageSize);
public sealed record GetRecipesQuery(GetRecipesParameters Parameters);
internal sealed record GetRecipesProjection(Guid Id, string Title, string? ImageUrl, int Difficulty, byte NumberOfServings);
public sealed record GetRecipesRecipeDto(Guid Id, string Title, string? ImageUrl, int Difficulty, byte NumberOfServings);
public sealed record GetRecipesResponse(int Page, int PageSize, int TotalCount, IEnumerable<HateoasResponse<GetRecipesRecipeDto>> Recipes)
    : PagedResult(Page, PageSize, TotalCount);

public sealed class GetRecipesQueryHandler(IAppDbContext dbContext, IHateoasBuilderFactory hateoasBuilderFactory)
{
    public async Task<HateoasResponse<GetRecipesResponse>> Handle(GetRecipesQuery query, CancellationToken cancellationToken)
    {
        GetRecipesParameters parameters = query.Parameters;
        IQueryable<Recipe> recipesQuery = dbContext.Recipes.AsNoTracking();
        if (parameters.Categories?.Ids.Count == 0)
        {
            recipesQuery = recipesQuery.Where(recipe =>
                recipe.Categories.Any(category => parameters.Categories.Ids.Contains(category.Value)));
        }

        if (parameters.Ingredients?.Ids.Count == 0)
        {
            recipesQuery = recipesQuery.Where(recipe =>
                recipe.Ingredients.Any(ingredient => parameters.Ingredients.Ids.Contains(ingredient.IngredientId.Value)));
        }

        int totalCount = await recipesQuery.CountAsync(cancellationToken);
        List<GetRecipesRecipeDto> recipes = await recipesQuery.SetPage(parameters)
            .Select(recipe => new GetRecipesRecipeDto(
                recipe.Id.Value, recipe.Title, recipe.ImageURL, (int)recipe.Difficulty, recipe.NumberOfServings))
            .ToListAsync(cancellationToken);
        return MapToResponse(recipes, parameters.Page, parameters.PageSize, totalCount);
    }

    private HateoasResponse<GetRecipesResponse> MapToResponse(
        IEnumerable<GetRecipesRecipeDto> recipes,
        int page,
        int pageSize,
        int totalCount)
    {
        HateoasCollectionResponseBuilder<GetRecipesRecipeDto> collectionBuilder =
            hateoasBuilderFactory.ForCollection(recipes);
        collectionBuilder.WithCollectionLink()
            .WithGet(LinkOptions.Create("GetRecipeById", HateoasRelConstants.Self, true),
                recipe => new { id = recipe.Id });

        GetRecipesResponse response = new(page, pageSize, totalCount, collectionBuilder.Build().Items);
        HateoasResponseBuilder<GetRecipesResponse> responseBuilder = hateoasBuilderFactory.ForItem(response);
        responseBuilder.AddPagedNavigation("GetRecipes", new { page, pageSize });
        responseBuilder.AddPost(LinkOptions.Create("CreateRecipe", HateoasRelConstants.Create, true), null);
        return responseBuilder.Build();
    }
}

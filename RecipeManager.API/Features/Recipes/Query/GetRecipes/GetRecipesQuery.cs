using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Features.Units;
using RecipeManager.Api.Persistance.Extensions;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Hateoas.Models;

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
internal sealed record GetRecipesProjection(
    Guid Id,
    string Title,
    string? ImageUrl,
    int Difficulty,
    byte NumberOfServings,
    string CreatedBy);
public sealed record GetRecipesRecipeDto(
    Guid Id,
    string Title,
    string? ImageUrl,
    int Difficulty,
    byte NumberOfServings,
    ItemActionDto Actions);
public sealed record GetRecipesResponse(
    int Page,
    int PageSize,
    int TotalCount,
    CollectionActionDto Actions,
    IEnumerable<GetRecipesRecipeDto> Recipes)
    : PagedResult(Page, PageSize, TotalCount);

public sealed class GetRecipesQueryHandler(IAppDbContext dbContext, ICurrentUser currentUser)
{
    public async Task<GetRecipesResponse> Handle(GetRecipesQuery query, CancellationToken cancellationToken)
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
        List<GetRecipesProjection> projections = await recipesQuery.SetPage(parameters)
            .Select(recipe => new GetRecipesProjection(
                recipe.Id.Value,
                recipe.Title,
                recipe.ImageURL,
                (int)recipe.Difficulty,
                recipe.NumberOfServings,
                recipe.CreatedBy))
            .ToListAsync(cancellationToken);
        List<GetRecipesRecipeDto> recipes = projections.Select(projection =>
        {
            bool canEdit = projection.CreatedBy == currentUser.Id;
            return new GetRecipesRecipeDto(
                projection.Id,
                projection.Title,
                projection.ImageUrl,
                projection.Difficulty,
                projection.NumberOfServings,
                new ItemActionDto(true, canEdit, canEdit));
        }).ToList();
        return new GetRecipesResponse(
            parameters.Page,
            parameters.PageSize,
            totalCount,
            new CollectionActionDto(true),
            recipes);
    }
}

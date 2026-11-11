using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Cookbooks;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Features.Units;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.Cookbooks.Query.GetCookbookById;

public sealed record GetCookbookByIdQuery(Guid CookbookId);

internal sealed record GetCookbookByIdProjection(
    Guid Id,
    string Title,
    string Description,
    string? CoverImageUrl,
    string CreatedBy);

public sealed record GetCookbookByIdRecipeDto(
    Guid Id,
    string Title,
    string? Description,
    string? ImageUrl,
    ItemActionDto Actions);

public sealed record GetCookbookByIdCategoryDto(
    Guid Id,
    string Name,
    IEnumerable<GetCookbookByIdRecipeDto> Recipes,
    IEnumerable<GetCookbookByIdCategoryDto> Subcategories);

public sealed record GetCookbookByIdResponse(
    Guid Id,
    string Title,
    string Description,
    string? CoverImageUrl,
    IEnumerable<GetCookbookByIdCategoryDto> Categories,
    ItemActionDto Actions);

public sealed class GetCookbookByIdQueryHandler(
    IAppDbContext dbContext,
    ICurrentUser currentUser)
{
    public async Task<Result<GetCookbookByIdResponse>> Handle(
        GetCookbookByIdQuery query,
        CancellationToken cancellationToken)
    {
        CookbookId id = new(query.CookbookId);
        GetCookbookByIdProjection? cookbook = await dbContext.Cookbooks
            .AsNoTracking()
            .Where(item => item.Id == id)
            .Select(item => new GetCookbookByIdProjection(
                item.Id.Value,
                item.Title,
                item.Description,
                item.CoverImageUrl,
                item.CreatedBy))
            .FirstOrDefaultAsync(cancellationToken);

        if (cookbook is null)
        {
            return new NotFoundError();
        }

        List<CookbookCategory> categories = await dbContext.CookbookCategories
            .AsNoTracking()
            .Where(category => category.Cookbook.Id == id)
            .Include(category => category.Recipes)
            .ToListAsync(cancellationToken);

        IEnumerable<RecipeId> recipeIds = categories.SelectMany(category => category.Recipes).Distinct();
        List<GetCookbookByIdRecipeDto> recipes = await dbContext.Recipes
            .AsNoTracking()
            .Where(recipe => recipeIds.Contains(recipe.Id))
            .Select(recipe => new GetCookbookByIdRecipeDto(
                recipe.Id.Value,
                recipe.Title,
                recipe.Description,
                recipe.ImageURL,
                new ItemActionDto(true, false, false)))
            .ToListAsync(cancellationToken);

        bool isCookbookCreator = cookbook.CreatedBy == currentUser.Id;
        return MapToResponse(cookbook, categories, recipes, isCookbookCreator);
    }

    private static GetCookbookByIdResponse MapToResponse(
        GetCookbookByIdProjection cookbook,
        IEnumerable<CookbookCategory> categories,
        IEnumerable<GetCookbookByIdRecipeDto> recipes,
        bool canEdit)
    {
        IEnumerable<CookbookCategory> rootCategories = categories.Where(category => category.ParentId is null);

        return new GetCookbookByIdResponse(
            cookbook.Id,
            cookbook.Title,
            cookbook.Description,
            cookbook.CoverImageUrl,
            rootCategories.Select(category => MapToCategoryDto(category, categories, recipes)),
            new ItemActionDto(true, canEdit, canEdit));
    }

    private static GetCookbookByIdCategoryDto MapToCategoryDto(
        CookbookCategory category,
        IEnumerable<CookbookCategory> allCategories,
        IEnumerable<GetCookbookByIdRecipeDto> recipes)
    {
        IEnumerable<CookbookCategory> subcategories = allCategories.Where(item => item.ParentId == category.Id);
        IEnumerable<GetCookbookByIdCategoryDto> subcategoryDtos = subcategories
            .Select(subcategory => MapToCategoryDto(subcategory, allCategories, recipes));
        IEnumerable<GetCookbookByIdRecipeDto> categoryRecipes = recipes
            .Where(recipe => category.Recipes.Contains(new RecipeId(recipe.Id)));

        return new GetCookbookByIdCategoryDto(category.Id.Value, category.Name, categoryRecipes, subcategoryDtos);
    }
}

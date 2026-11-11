using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Recipes.Enums;
using RecipeManager.Api.Persistance.Extensions;
using RecipeManager.Api.Features.Units;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Hateoas.Models;

namespace RecipeManager.Api.Features.RecipeCategories.Query.GetRecipeCategories;

public sealed record GetRecipeCategoriesRequest(int Page = 1, int PageSize = 10, int? CategoryType = null)
    : PagedParameters(Page, PageSize);

public sealed record GetRecipeCategoriesQuery(GetRecipeCategoriesRequest Request);

internal sealed record GetRecipeCategoriesProjection(Guid Id, string Name, RecipeCategoryType Type);

public sealed record GetRecipeCategoryDto(Guid Id, string Name, string Type, ItemActionDto Actions)
{
    internal static GetRecipeCategoryDto FromProjection(GetRecipeCategoriesProjection category, bool canEdit) =>
        new(category.Id, category.Name, category.Type.ToFriendlyString(), new ItemActionDto(true, canEdit, canEdit));
}

public sealed record GetRecipeCategoriesResponse(
    int Page,
    int PageSize,
    int TotalCount,
    CollectionActionDto Actions,
    IEnumerable<GetRecipeCategoryDto> Categories) : PagedResult(Page, PageSize, TotalCount);

public sealed class GetRecipeCategoriesQueryHandler(
    IAppDbContext dbContext,
    ICurrentUser currentUser)
{
    public async Task<GetRecipeCategoriesResponse> Handle(
        GetRecipeCategoriesQuery query,
        CancellationToken cancellationToken)
    {
        GetRecipeCategoriesRequest request = query.Request;
        IQueryable<RecipeCategory> categories = dbContext.RecipeCategories.AsNoTracking();
        if (request.CategoryType is not null)
        {
            categories = categories.Where(category => category.Type == (RecipeCategoryType)request.CategoryType);
        }

        int totalCount = await categories.CountAsync(cancellationToken);
        List<GetRecipeCategoriesProjection> projections = await categories.OrderBy(category => category.Name)
            .SetPage(request)
            .Select(category => new GetRecipeCategoriesProjection(category.Id.Value, category.Name, category.Type))
            .ToListAsync(cancellationToken);

        bool canEdit = currentUser.HasRole(UserRoles.Admin);
        IEnumerable<GetRecipeCategoryDto> categoryDtos = projections
            .Select(category => GetRecipeCategoryDto.FromProjection(category, canEdit));
        GetRecipeCategoriesResponse response = new(
            request.Page,
            request.PageSize,
            totalCount,
            new CollectionActionDto(canEdit),
            categoryDtos);
        return response;
    }
}

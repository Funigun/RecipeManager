using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Cookbooks;
using RecipeManager.Api.Features.Units;
using RecipeManager.Api.Persistance.Extensions;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Hateoas.Models;

namespace RecipeManager.Api.Features.Cookbooks.Query.GetCookbooks;

public sealed record GetCookbooksParameters(
    string SortBy = "title",
    bool IsAscending = true,
    int Page = 1,
    int PageSize = 10) : PagedParameters(Page, PageSize);

public sealed record GetCookbooksQuery(GetCookbooksParameters Parameters);

internal sealed record GetCookbooksProjection(Guid Id, string Title, string? CoverImageUrl);

public sealed record GetCookbooksResponse(
    int Page,
    int PageSize,
    int TotalCount,
    CollectionActionDto Actions,
    IEnumerable<GetCookbooksItemResponse> Cookbooks) : PagedResult(Page, PageSize, TotalCount);

public sealed record GetCookbooksItemResponse(Guid Id, string Title, string? CoverImageUrl, ItemActionDto Actions);

public sealed class GetCookbooksQueryHandler(
    IAppDbContext dbContext,
    ICurrentUser currentUser)
{
    public async Task<Result<GetCookbooksResponse>> Handle(
        GetCookbooksQuery query,
        CancellationToken cancellationToken)
    {
        GetCookbooksParameters parameters = query.Parameters;
        IQueryable<Cookbook> cookbooksQuery = dbContext.Cookbooks
            .Where(cookbook => cookbook.CreatedBy == currentUser.Id);
        int totalCount = await cookbooksQuery.CountAsync(cancellationToken);

        cookbooksQuery = parameters.SortBy.ToLower() switch
        {
            "title" => parameters.IsAscending
                ? cookbooksQuery.OrderBy(item => item.Title)
                : cookbooksQuery.OrderByDescending(item => item.Title),
            "createdat" => parameters.IsAscending
                ? cookbooksQuery.OrderBy(item => item.CreatedOn)
                : cookbooksQuery.OrderByDescending(item => item.CreatedOn),
            _ => cookbooksQuery.OrderBy(item => item.Title)
        };

        List<GetCookbooksProjection> projections = await cookbooksQuery
            .SetPage(parameters)
            .Select(item => new GetCookbooksProjection(item.Id.Value, item.Title, item.CoverImageUrl))
            .ToListAsync(cancellationToken);
        List<GetCookbooksItemResponse> items = projections
            .Select(item => new GetCookbooksItemResponse(
                item.Id,
                item.Title,
                item.CoverImageUrl,
                new ItemActionDto(true, true, true)))
            .ToList();

        GetCookbooksResponse response = new(
            parameters.Page,
            parameters.PageSize,
            totalCount,
            new CollectionActionDto(true),
            items);
        return response;
    }
}

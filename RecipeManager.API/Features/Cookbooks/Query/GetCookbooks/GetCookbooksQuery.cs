using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Cookbooks;
using RecipeManager.Api.Persistance.Extensions;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Hateoas.Builder;
using RecipeManager.Api.Shared.Hateoas.Common;
using RecipeManager.Api.Shared.Hateoas.Models;
using LinkOptions = RecipeManager.Api.Shared.Hateoas.Models.LinkOptions;

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
    IEnumerable<HateoasResponse<GetCookbooksItemResponse>> Cookbooks) : PagedResult(Page, PageSize, TotalCount);

public sealed record GetCookbooksItemResponse(Guid Id, string Title, string? CoverImageUrl);

public sealed class GetCookbooksQueryHandler(
    IAppDbContext dbContext,
    ICurrentUser currentUser,
    IHateoasBuilderFactory hateoasBuilderFactory)
{
    public async Task<Result<HateoasResponse<GetCookbooksResponse>>> Handle(
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
            .Select(item => new GetCookbooksItemResponse(item.Id, item.Title, item.CoverImageUrl))
            .ToList();

        HateoasCollectionResponseBuilder<GetCookbooksItemResponse> cookbooksBuilder =
            hateoasBuilderFactory.ForCollection(items);
        cookbooksBuilder.WithCollectionLink()
            .WithGet(
                LinkOptions.Create("GetCookbookById", HateoasRelConstants.Self, true),
                cookbook => new { id = cookbook.Id });

        GetCookbooksResponse response = new(
            parameters.Page,
            parameters.PageSize,
            totalCount,
            cookbooksBuilder.Build().Items);
        HateoasResponseBuilder<GetCookbooksResponse> responseBuilder = hateoasBuilderFactory.ForItem(response);
        responseBuilder.AddPost(LinkOptions.Create("CreateCookbook", HateoasRelConstants.Create, true), null);

        return responseBuilder.Build();
    }
}

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Cookbooks;
using RecipeManager.Api.Persistance.Extensions;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Api.Shared.Hateoas.Builder;
using RecipeManager.Api.Shared.Hateoas.Common;
using RecipeManager.Api.Shared.Hateoas.Models;
using LinkOptions = RecipeManager.Api.Shared.Hateoas.Models.LinkOptions;

namespace RecipeManager.Api.Features.Cookbooks;

public static class GetCookbooks
{
    public sealed record GetCookbooksParameters(string SortBy = "title", bool IsAscending = true, int Page = 1, int PageSize = 10) : PagedParameters(Page, PageSize);

    public sealed record CoookbookDto(Guid Id, string Title, string? CoverImageUrl);

    public sealed record Response(int Page, int PageSize, int TotalCount, IEnumerable<HateoasResponse<CoookbookDto>> Cookbooks) : PagedResult(Page, PageSize, TotalCount);

    [GroupEndpoint("Cookbooks")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardGet<Response>("/", Handler)
                     .WithName("GetCookbooks")
                     .WithDescription("Get a list of cookbooks");
        }
    }

    public static async Task<Results<Ok<HateoasResponse<Response>>, BadRequest>> Handler([AsParameters] GetCookbooksParameters parameters, [FromServices] IAppDbContext dbContext, [FromServices] ICurrentUser currentUser, [FromServices] IHateoasBuilderFactory hateoasBuilderFactory, CancellationToken cancellationToken)
    {
        IQueryable<Cookbook> query = dbContext.Cookbooks.Where(cookbook => cookbook.CreatedBy == currentUser.Id);

        int totalCount = await query.CountAsync(cancellationToken);

        query = parameters.SortBy.ToLower() switch
        {
            "title" => parameters.IsAscending ? query.OrderBy(c => c.Title) : query.OrderByDescending(c => c.Title),
            "createdat" => parameters.IsAscending ? query.OrderBy(c => c.CreatedOn) : query.OrderByDescending(c => c.CreatedOn),
            _ => query.OrderBy(c => c.Title)
        };

        List<CoookbookDto> cookbooks = await query.SetPage(parameters)
                                                  .Select(c => new CoookbookDto(c.Id.Value, c.Title, c.CoverImageUrl))
                                                  .ToListAsync(cancellationToken);

        HateoasCollectionResponseBuilder<CoookbookDto> cookbooksBuilder = hateoasBuilderFactory.ForCollection(cookbooks);

        cookbooksBuilder.WithCollectionLink()
                        .WithGet(LinkOptions.Create("GetCookbookById", HateoasRelConstants.Self, true), cookbook => new { id = cookbook.Id });

        Response response = new(parameters.Page, parameters.PageSize, totalCount, cookbooksBuilder.Build().Items);

        HateoasResponseBuilder<Response> responseBuilder = hateoasBuilderFactory.ForItem(response);
        responseBuilder.AddPost(LinkOptions.Create("CreateCookbook", HateoasRelConstants.Create, true), null);

        return TypedResults.Ok(responseBuilder.Build());
    }
}

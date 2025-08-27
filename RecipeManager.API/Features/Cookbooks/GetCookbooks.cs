using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Api.Shared.Hateoas.Builder;
using RecipeManager.Api.Shared.Hateoas.Common;
using RecipeManager.Api.Shared.Hateoas.Models;
using LinkOptions = RecipeManager.Api.Shared.Hateoas.Models.LinkOptions;

namespace RecipeManager.Api.Features.Cookbooks;

public static class GetCookbooks
{
    public sealed record CoookbookDto(Guid Id, string Title);

    public sealed record Response(IEnumerable<HateoasResponse<CoookbookDto>> Cookbooks);

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

    public static async Task<Results<Ok<HateoasResponse<Response>>, BadRequest>> Handler(IAppDbContext dbContext, ICurrentUser currentUser, IHateoasBuilderFactory hateoasBuilderFactory, CancellationToken cancellationToken)
    {
        List<CoookbookDto> cookbooks = await dbContext.Cookbooks
                                                      .Where(cookbook => cookbook.CreatedBy == currentUser.Id)
                                                      .Select(c => new CoookbookDto(c.Id.Value, c.Title))
                                                      .ToListAsync(cancellationToken);

        HateoasCollectionResponseBuilder<CoookbookDto> cookbooksBuilder = hateoasBuilderFactory.ForCollection(cookbooks);

        cookbooksBuilder.WithCollectionLink()
                        .WithGet(LinkOptions.Create("GetCookbookById", HateoasRelConstants.Self, true), cookbook => new { id = cookbook.Id });

        Response response = new(cookbooksBuilder.Build().Items);

        HateoasResponseBuilder<Response> responseBuilder = hateoasBuilderFactory.ForItem(response);
        responseBuilder.AddPost(LinkOptions.Create("CreateCookbook", HateoasRelConstants.Create, true), null);

        return TypedResults.Ok(responseBuilder.Build());
    }
}

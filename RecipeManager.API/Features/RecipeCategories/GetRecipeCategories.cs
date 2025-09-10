using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Api.Shared.Hateoas.Builder;
using RecipeManager.Api.Shared.Hateoas.Common;
using RecipeManager.Api.Shared.Hateoas.Models;
using RecipeManager.Shared.Contracts.Authorization;
using LinkOptions = RecipeManager.Api.Shared.Hateoas.Models.LinkOptions;

namespace RecipeManager.Api.Features.RecipeCategories;

public static class GetRecipeCategories
{
    public sealed record Response(Guid Id, string Name);

    [GroupEndpoint("RecipeCategories")]
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardGet<Response>(string.Empty, Handler)
                     .WithName("GetRecipeCategories")
                     .WithDescription("Gets all recipe categories");
        }
    }

    internal static async Task<Results<Ok<HateoasCollectionResponse<Response>>, NotFound>> Handler([FromServices] ICurrentUser currentUser, IAppDbContext dbContext, [FromServices] IHateoasBuilderFactory hateoasBuilderFactory, CancellationToken cancellationToken)
    {
        IEnumerable<Response> results = await dbContext.RecipeCategories.AsNoTracking()
                                                                        .OrderBy(category => category.Name)
                                                                        .Select(category => new Response(category.Id, category.Name))
                                                                        .ToListAsync(cancellationToken);

        bool isActionAllowed = currentUser.HasRole(UserRoles.Admin);

        HateoasCollectionResponseBuilder<Response> collectionBuilder = hateoasBuilderFactory.ForCollection(results);

        if (isActionAllowed)
        {
            collectionBuilder
                .WithCollectionLink()
                    .WithDelete(LinkOptions.Create("DeleteRecipeCategory", HateoasRelConstants.Delete, isActionAllowed), unit => new { categoryId = unit.Id })
                .AddPost(LinkOptions.Create("CreateRecipeCategory", HateoasRelConstants.Create, isActionAllowed), null);
        }

        return TypedResults.Ok(collectionBuilder.Build());
    }
}

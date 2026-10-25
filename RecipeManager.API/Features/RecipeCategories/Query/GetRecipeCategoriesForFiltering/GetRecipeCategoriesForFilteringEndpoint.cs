using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.RecipeCategories.Query.GetRecipeCategoriesForFiltering;

public sealed class GetRecipeCategoriesForFilteringEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardGet<GetRecipeCategoriesForFilteringResponse>(RecipeCategoriesGroup.FilteringRoute, Handle)
                 .WithName("GetRecipeCategoriesForFiltering")
                 .WithDescription("Get recipe categories grouped by category type for filtering purposes.")
                 .WithTags(RecipeCategoriesGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Ok<GetRecipeCategoriesForFilteringResponse>> Handle(
        [FromServices] GetRecipeCategoriesForFilteringQueryHandler queryHandler,
        CancellationToken cancellationToken)
    {
        GetRecipeCategoriesForFilteringResponse response = await queryHandler.Handle(cancellationToken);
        return TypedResults.Ok(response);
    }
}

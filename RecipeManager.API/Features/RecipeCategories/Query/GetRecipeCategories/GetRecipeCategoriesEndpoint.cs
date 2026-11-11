using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.RecipeCategories.Query.GetRecipeCategories;

public sealed class GetRecipeCategoriesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardGet<GetRecipeCategoryDto>(RecipeCategoriesGroup.Route, Handle)
                 .WithName("GetRecipeCategories")
                 .WithDescription("Gets all recipe categories")
                 .WithTags(RecipeCategoriesGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Ok<GetRecipeCategoriesResponse>> Handle(
        [AsParameters] GetRecipeCategoriesRequest request,
        [FromServices] GetRecipeCategoriesQueryHandler queryHandler,
        CancellationToken cancellationToken)
    {
        GetRecipeCategoriesResponse response = await queryHandler.Handle(
            new GetRecipeCategoriesQuery(request), cancellationToken);
        return TypedResults.Ok(response);
    }
}

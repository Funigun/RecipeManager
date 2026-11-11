using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Recipes.Query.GetRecipes;

public sealed class GetRecipesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardGet<GetRecipesResponse>(RecipesGroup.Route, Handle)
            .WithName("GetRecipes")
            .WithDescription("Get a paginated list of recipes")
            .WithTags(RecipesGroup.Tag)
            .RequireAuthorization("RecipeManagerPolicy")
            .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Ok<GetRecipesResponse>> Handle(
        [AsParameters] GetRecipesParameters parameters,
        [FromServices] GetRecipesQueryHandler queryHandler,
        CancellationToken cancellationToken)
    {
        GetRecipesResponse response =
            await queryHandler.Handle(new GetRecipesQuery(parameters), cancellationToken);
        return TypedResults.Ok(response);
    }
}

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Recipes.Query.GetRecipeForDropdownModel;

public sealed class GetRecipeForDropdownModelEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardGet<GetRecipeForDropdownModelResponse>(RecipesGroup.DropdownRoute, Handle)
            .WithName("GetRecipeForDropdownModel")
            .WithDescription("Get a recipe for dropdown model")
            .WithTags(RecipesGroup.Tag)
            .RequireAuthorization("RecipeManagerPolicy")
            .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Ok<IEnumerable<GetRecipeForDropdownModelResponse>>> Handle(
        [FromQuery] string recipeName,
        [FromQuery] int numberOfRecipesToLoad,
        [FromServices] GetRecipeForDropdownModelQueryHandler queryHandler,
        CancellationToken cancellationToken)
    {
        GetRecipeForDropdownModelParameters parameters = new(recipeName, numberOfRecipesToLoad);
        IEnumerable<GetRecipeForDropdownModelResponse> response =
            await queryHandler.Handle(new GetRecipeForDropdownModelQuery(parameters), cancellationToken);
        return TypedResults.Ok(response);
    }
}

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application;
using RecipeManager.Api.Presentation;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Recipes.Command.UpdateRecipe;

public sealed class UpdateRecipeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardPut<UpdateRecipeRequest>(RecipesGroup.RecipeByRecipeIdRoute, Handle)
            .WithName("UpdateRecipe")
            .WithDescription("Updates existing recipe")
            .WithTags(RecipesGroup.Tag)
            .RequireAuthorization("RecipeManagerPolicy")
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Results<NoContent, ValidationProblem, ProblemHttpResult>> Handle(
        Guid recipeId,
        [FromBody] UpdateRecipeRequest request,
        [FromServices] UpdateRecipeCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        Result<UpdateRecipeResponse> result =
            await commandHandler.Handle(new UpdateRecipeCommand(recipeId, request), cancellationToken);
        return result.ToHttpResult(_ => TypedResults.NoContent());
    }
}

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application;
using RecipeManager.Api.Presentation;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Recipes.Command.DeleteRecipe;

public sealed class DeleteRecipeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardDelete<DeleteRecipeCommand>(RecipesGroup.RecipeByRecipeIdRoute, Handle)
            .WithName("DeleteRecipe")
            .WithDescription("Delete a recipe by id")
            .WithTags(RecipesGroup.Tag)
            .RequireAuthorization("RecipeManagerPolicy")
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Results<NoContent, ValidationProblem, ProblemHttpResult>> Handle(
        Guid recipeId,
        [FromServices] DeleteRecipeCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        Result<DeleteRecipeResponse> result =
            await commandHandler.Handle(new DeleteRecipeCommand(recipeId), cancellationToken);
        return result.ToHttpResult(_ => TypedResults.NoContent());
    }
}

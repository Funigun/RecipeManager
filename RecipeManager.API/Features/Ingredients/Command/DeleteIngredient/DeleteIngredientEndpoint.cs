using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application;
using RecipeManager.Api.Presentation;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Ingredients.Command.DeleteIngredient;

public sealed class DeleteIngredientEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardDelete<DeleteIngredientCommand>(IngredientsGroup.IngredientByIdRoute, Handle)
                 .WithName("DeleteIngredient")
                 .WithDescription("Deletes an ingredient")
                 .WithTags(IngredientsGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status401Unauthorized)
                 .ProducesProblem(StatusCodes.Status403Forbidden);
    }

    private static async Task<Results<NoContent, ValidationProblem, ProblemHttpResult>> Handle(
        Guid ingredientId,
        [FromServices] DeleteIngredientCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        Result<RecipeManager.Api.Domain.Ingredients.IngredientId> result = await commandHandler.Handle(
            new DeleteIngredientCommand(ingredientId),
            cancellationToken);
        return result.ToHttpResult(_ => TypedResults.NoContent());
    }
}

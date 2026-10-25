using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application;
using RecipeManager.Api.Presentation;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Ingredients.Command.UpdateIngredient;

public sealed class UpdateIngredientEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardPut<UpdateIngredientRequest>(IngredientsGroup.IngredientByIdRoute, Handle)
                 .WithName("UpdateIngredient")
                 .WithDescription("Updates an existing ingredient")
                 .WithTags(IngredientsGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status401Unauthorized)
                 .ProducesProblem(StatusCodes.Status403Forbidden);
    }

    private static async Task<Results<NoContent, ValidationProblem, ProblemHttpResult>> Handle(
        Guid ingredientId,
        [FromBody] UpdateIngredientRequest request,
        [FromServices] UpdateIngredientCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        Result<RecipeManager.Api.Domain.Ingredients.IngredientId> result = await commandHandler.Handle(
            new UpdateIngredientCommand(ingredientId, request),
            cancellationToken);
        return result.ToHttpResult(_ => TypedResults.NoContent());
    }
}

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application;
using RecipeManager.Api.Presentation;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Ingredients.Command.GenerateShoppingList;

public sealed class GenerateShoppingListEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardPut<GenerateShoppingListRequest>(IngredientsGroup.GenerateShoppingListRoute, Handle)
                 .WithName("GenerateShoppingList")
                 .WithDescription("Generates a shopping list based on the provided recipe IDs.")
                 .WithTags(IngredientsGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Results<Ok<GenerateShoppingListResponse>, ValidationProblem, ProblemHttpResult>> Handle(
        [FromBody] GenerateShoppingListRequest request,
        [FromServices] GenerateShoppingListCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        Result<GenerateShoppingListResponse> result = await commandHandler.Handle(
            new GenerateShoppingListCommand(request),
            cancellationToken);
        return result.ToHttpResult(TypedResults.Ok);
    }
}

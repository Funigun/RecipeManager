using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application;
using RecipeManager.Api.Presentation;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Ingredients.Command.CreateIngredient;

public sealed class CreateIngredientEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardPost<CreateIngredientRequest, CreateIngredientResponse>(IngredientsGroup.Route, Handle)
                 .WithName("CreateIngredient")
                 .WithDescription("Creates a new ingredient")
                 .WithTags(IngredientsGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Results<Created<CreateIngredientResponse>, ValidationProblem, ProblemHttpResult>> Handle(
        [FromBody] CreateIngredientRequest request,
        [FromServices] CreateIngredientCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        Result<CreateIngredientResponse> result = await commandHandler.Handle(
            new CreateIngredientCommand(request),
            cancellationToken);

        return result.ToHttpResult(response =>
            TypedResults.Created(IngredientsGroup.IngredientById(response.Id.Value), response));
    }
}

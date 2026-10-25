using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application;
using RecipeManager.Api.Presentation;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Recipes.Command.CreateRecipe;

public sealed class CreateRecipeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardPost<CreateRecipeRequest, CreateRecipeResponse>(RecipesGroup.Route, Handle)
            .WithName("CreateRecipe")
            .WithDescription("Creates a new recipe")
            .WithTags(RecipesGroup.Tag)
            .RequireAuthorization("RecipeManagerPolicy")
            .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Results<Created<CreateRecipeResponse>, ValidationProblem, ProblemHttpResult>> Handle(
        [FromBody] CreateRecipeRequest request,
        [FromServices] CreateRecipeCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        Result<CreateRecipeResponse> result = await commandHandler.Handle(new CreateRecipeCommand(request), cancellationToken);
        return result.ToHttpResult(response => TypedResults.Created(RecipesGroup.RecipeById(response.Id), response));
    }
}

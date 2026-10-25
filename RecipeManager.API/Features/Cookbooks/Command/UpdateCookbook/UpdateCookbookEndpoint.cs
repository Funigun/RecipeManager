using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application;
using RecipeManager.Api.Presentation;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Cookbooks.Command.UpdateCookbook;

public sealed class UpdateCookbookEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardPut<UpdateCookbookRequest>(CookbooksGroup.CookbookByIdRoute, Handle)
                 .WithName("UpdateCookbook")
                 .WithDescription("Updates a cookbook")
                 .WithTags(CookbooksGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status401Unauthorized)
                 .ProducesProblem(StatusCodes.Status403Forbidden);
    }

    private static async Task<Results<Ok, ValidationProblem, ProblemHttpResult>> Handle(
        Guid cookbookId,
        [FromBody] UpdateCookbookRequest request,
        [FromServices] UpdateCookbookCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        Result<Guid> result = await commandHandler.Handle(
            new UpdateCookbookCommand(cookbookId, request),
            cancellationToken);
        return result.ToHttpResult(_ => TypedResults.Ok());
    }
}

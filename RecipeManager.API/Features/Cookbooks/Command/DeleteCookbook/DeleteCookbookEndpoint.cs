using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application;
using RecipeManager.Api.Domain.Cookbooks;
using RecipeManager.Api.Presentation;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Cookbooks.Command.DeleteCookbook;

public sealed class DeleteCookbookEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardDelete<DeleteCookbookCommand>(CookbooksGroup.CookbookByIdRoute, Handle)
                 .WithName("DeleteCookbook")
                 .WithDescription("Deletes a cookbook")
                 .WithTags(CookbooksGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status403Forbidden)
                 .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Results<NoContent, ValidationProblem, ProblemHttpResult>> Handle(
        Guid cookbookId,
        [FromServices] DeleteCookbookCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        Result<CookbookId> result = await commandHandler.Handle(new DeleteCookbookCommand(cookbookId), cancellationToken);
        return result.ToHttpResult(_ => TypedResults.NoContent());
    }
}

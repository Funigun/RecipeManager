using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application;
using RecipeManager.Api.Presentation;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Cookbooks.Command.CreateCookbook;

public sealed class CreateCookbookEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardPost<CreateCookbookRequest, CreateCookbookResponse>(CookbooksGroup.Route, Handle)
                 .WithName("CreateCookbook")
                 .WithDescription("Creates a new cookbook")
                 .WithTags(CookbooksGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Results<Ok<CreateCookbookResponse>, ValidationProblem, ProblemHttpResult>> Handle(
        CreateCookbookRequest request,
        [FromServices] CreateCookbookCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        Result<CreateCookbookResponse> result = await commandHandler.Handle(new CreateCookbookCommand(request), cancellationToken);
        return result.ToHttpResult(TypedResults.Ok);
    }
}

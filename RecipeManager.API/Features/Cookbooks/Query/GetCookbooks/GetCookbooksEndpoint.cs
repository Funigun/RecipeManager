using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application;
using RecipeManager.Api.Presentation;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Api.Shared.Hateoas.Models;

namespace RecipeManager.Api.Features.Cookbooks.Query.GetCookbooks;

public sealed class GetCookbooksEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardGet<HateoasResponse<GetCookbooksResponse>>(CookbooksGroup.Route, Handle)
                 .WithName("GetCookbooks")
                 .WithDescription("Get a list of cookbooks")
                 .WithTags(CookbooksGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Results<Ok<HateoasResponse<GetCookbooksResponse>>, ValidationProblem, ProblemHttpResult>> Handle(
        [AsParameters] GetCookbooksParameters parameters,
        [FromServices] GetCookbooksQueryHandler queryHandler,
        CancellationToken cancellationToken)
    {
        Result<HateoasResponse<GetCookbooksResponse>> result = await queryHandler.Handle(
            new GetCookbooksQuery(parameters),
            cancellationToken);
        return result.ToHttpResult(TypedResults.Ok);
    }
}

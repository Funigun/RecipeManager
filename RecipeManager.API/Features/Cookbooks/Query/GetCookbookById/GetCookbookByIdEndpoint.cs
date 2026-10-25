using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application;
using RecipeManager.Api.Presentation;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Api.Shared.Hateoas.Models;

namespace RecipeManager.Api.Features.Cookbooks.Query.GetCookbookById;

public sealed class GetCookbookByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardGet<HateoasResponse<GetCookbookByIdResponse>>(CookbooksGroup.CookbookByIdRoute, Handle)
                 .WithName("GetCookbookById")
                 .WithDescription("Gets a cookbook by its id")
                 .WithTags(CookbooksGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Results<Ok<HateoasResponse<GetCookbookByIdResponse>>, ValidationProblem, ProblemHttpResult>> Handle(
        Guid cookbookId,
        [FromServices] GetCookbookByIdQueryHandler queryHandler,
        CancellationToken cancellationToken)
    {
        Result<HateoasResponse<GetCookbookByIdResponse>> result = await queryHandler.Handle(
            new GetCookbookByIdQuery(cookbookId),
            cancellationToken);
        return result.ToHttpResult(TypedResults.Ok);
    }
}

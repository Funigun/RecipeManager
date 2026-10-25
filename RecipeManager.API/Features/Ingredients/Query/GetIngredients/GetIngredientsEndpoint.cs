using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application;
using RecipeManager.Api.Presentation;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Api.Shared.Hateoas.Models;

namespace RecipeManager.Api.Features.Ingredients.Query.GetIngredients;

public sealed class GetIngredientsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardGet<GetIngredientsResponse>(IngredientsGroup.Route, Handle)
                 .WithName("GetIngredients")
                 .WithDescription("Gets a paginated list of ingredients")
                 .WithTags(IngredientsGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Results<Ok<HateoasResponse<GetIngredientsResponse>>, ValidationProblem, ProblemHttpResult>> Handle(
        [AsParameters] GetIngredientsFilterParameters filter,
        [FromServices] GetIngredientsQueryHandler queryHandler,
        CancellationToken cancellationToken)
    {
        Result<HateoasResponse<GetIngredientsResponse>> result = await queryHandler.Handle(
            new GetIngredientsQuery(filter),
            cancellationToken);
        return result.ToHttpResult(TypedResults.Ok);
    }
}

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application;
using RecipeManager.Api.Presentation;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Api.Shared.Hateoas.Models;

namespace RecipeManager.Api.Features.Recipes.Query.GetRecipeById;

public sealed class GetRecipeByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardGet<GetRecipeByIdResponse>(RecipesGroup.RecipeByIdRoute, Handle)
            .WithName("GetRecipeById")
            .WithDescription("Get a recibe by id")
            .WithTags(RecipesGroup.Tag)
            .RequireAuthorization("RecipeManagerPolicy")
            .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Results<Ok<HateoasResponse<GetRecipeByIdResponse>>, ValidationProblem, ProblemHttpResult>> Handle(
        Guid id,
        [FromServices] GetRecipeByIdQueryHandler queryHandler,
        CancellationToken cancellationToken)
    {
        Result<HateoasResponse<GetRecipeByIdResponse>> result =
            await queryHandler.Handle(new GetRecipeByIdQuery(id), cancellationToken);
        return result.ToHttpResult(TypedResults.Ok);
    }
}

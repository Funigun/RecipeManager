using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application;
using RecipeManager.Api.Presentation;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Api.Shared.Hateoas.Models;

namespace RecipeManager.Api.Features.Ingredients.Query.GetIngredientById;

public sealed class GetIngredientByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardGet<GetIngredientByIdResponse>(IngredientsGroup.IngredientByIdRoute, Handle)
                 .WithName("GetIngredientById")
                 .WithDescription("Gets an ingredient by its ID")
                 .WithTags(IngredientsGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Results<Ok<HateoasResponse<GetIngredientByIdResponse>>, ValidationProblem, ProblemHttpResult>> Handle(
        Guid ingredientId,
        [FromServices] GetIngredientByIdQueryHandler queryHandler,
        CancellationToken cancellationToken)
    {
        Result<HateoasResponse<GetIngredientByIdResponse>> result = await queryHandler.Handle(
            new GetIngredientByIdQuery(ingredientId),
            cancellationToken);
        return result.ToHttpResult(TypedResults.Ok);
    }
}

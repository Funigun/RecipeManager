using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.IngredientCategories.Query.GetIngredientCategories;

public sealed class GetIngredientCategoriesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardGet<GetIngredientCategoriesCollectionResponse>(IngredientCategoryGroup.Route, Handle)
                 .WithName("GetIngredientCategories")
                 .WithDescription("Gets all ingredient categories")
                 .WithTags(IngredientCategoryGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Ok<GetIngredientCategoriesCollectionResponse>> Handle(
        [FromServices] GetIngredientCategoriesQueryHandler queryHandler,
        CancellationToken cancellationToken)
    {
        GetIngredientCategoriesCollectionResponse response = await queryHandler.Handle(cancellationToken);
        return TypedResults.Ok(response);
    }
}

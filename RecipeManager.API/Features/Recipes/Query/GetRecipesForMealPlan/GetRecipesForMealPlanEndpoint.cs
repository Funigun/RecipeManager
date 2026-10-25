using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Recipes.Query.GetRecipesForMealPlan;

public sealed class GetRecipesForMealPlanEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardGet<GetRecipesForMealPlanResponse>(RecipesGroup.MealPlanRoute, Handle)
            .WithName("GetRecipesForMealPlan")
            .WithDescription("Get recipes for meal plan")
            .WithTags(RecipesGroup.Tag)
            .RequireAuthorization("RecipeManagerPolicy")
            .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Ok<IEnumerable<GetRecipesForMealPlanResponse>>> Handle(
        [AsParameters] GetRecipesForMealPlanParameters parameters,
        [FromServices] GetRecipesForMealPlanQueryHandler queryHandler,
        CancellationToken cancellationToken)
    {
        IEnumerable<GetRecipesForMealPlanResponse> response =
            await queryHandler.Handle(new GetRecipesForMealPlanQuery(parameters), cancellationToken);
        return TypedResults.Ok(response);
    }
}

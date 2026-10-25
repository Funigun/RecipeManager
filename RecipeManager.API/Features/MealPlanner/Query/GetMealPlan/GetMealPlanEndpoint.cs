using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.MealPlanner.Query.GetMealPlan;

public sealed class GetMealPlanEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardGet<GetMealPlanResponse>(MealPlanGroup.MonthlyRoute, Handle)
                 .WithName("GetMealPlanForMonth")
                 .WithDescription("Returns meal plan for month including days before/after firts/last day of month to present whole week")
                 .WithTags(MealPlanGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Ok<GetMealPlanResponse>> Handle(
        [AsParameters] GetMealPlanRequest request,
        [FromServices] GetMealPlanQueryHandler queryHandler,
        CancellationToken cancellationToken)
    {
        GetMealPlanResponse response = await queryHandler.Handle(new GetMealPlanQuery(request), cancellationToken);
        return TypedResults.Ok(response);
    }
}

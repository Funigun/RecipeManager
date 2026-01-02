using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.MealPlanner;

public class MealPlanGroup : IGroupEndpoint
{
    public string GroupName { get; } = "MealPlans";

    public void Configure(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.WithDescription("Endpoints for managing meal plans")
                    .WithTags("Meal Plans")
                    .RequireAuthorization("RecipeManagerPolicy")
                    .ProducesProblem(StatusCodes.Status401Unauthorized);
    }
}

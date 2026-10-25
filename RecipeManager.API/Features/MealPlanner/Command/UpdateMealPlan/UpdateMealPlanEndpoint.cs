using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Application;
using RecipeManager.Api.Presentation;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.MealPlanner.Command.UpdateMealPlan;

public sealed class UpdateMealPlanEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapStandardPut<UpdateMealPlanRequest>(MealPlanGroup.Route, Handle)
                 .WithName("UpdateMealPlan")
                 .WithDescription("Updates existing meal plans or creates new ones.")
                 .WithTags(MealPlanGroup.Tag)
                 .RequireAuthorization("RecipeManagerPolicy")
                 .ProducesProblem(StatusCodes.Status401Unauthorized)
                 .ProducesProblem(StatusCodes.Status403Forbidden);
    }

    private static async Task<Results<Ok, ValidationProblem, ProblemHttpResult>> Handle(
        UpdateMealPlanRequest request,
        [FromServices] UpdateMealPlanCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        Result<bool> result = await commandHandler.Handle(new UpdateMealPlanCommand(request), cancellationToken);
        return result.ToHttpResult(_ => TypedResults.Ok());
    }
}

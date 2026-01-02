using RecipeManager.UI.Blazor.Components.Common;
using RecipeManager.UI.Blazor.Features.MealPlan.Models;

namespace RecipeManager.UI.Blazor.Features.MealPlan.Services;

public interface IMealPlanService
{
    ApiResponseBody ResponseBody { get; }

    Task<MealPlannerMonth> GetMonthlyMealPlan(DateTimeOffset day);

    Task UpdateMealPlan(MealPlanForUpdateModel mealPlanForUpdate);
}

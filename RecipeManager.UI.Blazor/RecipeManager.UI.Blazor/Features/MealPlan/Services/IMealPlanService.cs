using RecipeManager.UI.Blazor.Features.MealPlan.Models;

namespace RecipeManager.UI.Blazor.Features.MealPlan.Services;

public interface IMealPlanService
{
    Task<MealPlannerMonth> GetMonthlyMealPlan(DateTimeOffset day);
}

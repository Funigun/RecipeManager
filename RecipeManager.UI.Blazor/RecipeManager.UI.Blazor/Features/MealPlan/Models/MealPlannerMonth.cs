namespace RecipeManager.UI.Blazor.Features.MealPlan.Models;

public class MealPlannerMonth
{
    public ICollection<MealPlannerWeek> Weeks { get; set; } = [];
}

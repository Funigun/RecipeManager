namespace RecipeManager.UI.Blazor.Features.MealPlan.Models;

public class MealPlannerWeek
{
    public ICollection<MealPlannerDay> Days { get; set; } = [];
}

using RecipeManager.UI.Blazor.Features.Recipes;

namespace RecipeManager.UI.Blazor.Features.MealPlan.Models;

public class MealPlannerMonth
{
    public ICollection<MealPlannerWeek> Weeks { get; set; } = [];

    public void AddRecipe(RecipeForMealPlanModel recipe)
    {
        foreach (MealPlannerWeek week in Weeks)
        {
            week.AddRecipe(recipe);
        }
    }
}

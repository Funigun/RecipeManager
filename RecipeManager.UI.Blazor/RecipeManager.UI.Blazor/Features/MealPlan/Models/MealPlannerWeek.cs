using RecipeManager.UI.Blazor.Features.Recipes;

namespace RecipeManager.UI.Blazor.Features.MealPlan.Models;

public class MealPlannerWeek
{
    public ICollection<MealPlannerDay> Days { get; set; } = [];

    public void AddRecipe(RecipeForMealPlanModel recipe)
    {
        foreach (MealPlannerDay day in Days)
        {
            if (day.IsSelected && !day.Recipes.Any(r => r.Id == recipe.Id))
            {
                day.AddRecipe(recipe);
            }
        }
    }

    public IEnumerable<MealPlannerDay> GetPlansToUpdate()
    {
        return Days.Where(day => day.HasChanges).ToList();
    }
}

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

    public IEnumerable<MealPlanForUpdateModel> GetPlansToUpdate()
    {
        return Weeks.SelectMany(week => week.GetPlansToUpdate()).Select(plan => new MealPlanForUpdateModel
        (
            plan.Id == Guid.Empty ? null : plan.Id,
            plan.Date,
            plan.Recipes.Select(recipe => recipe.Id).ToList()
        ));
    }
}

using RecipeManager.UI.Blazor.Features.Recipes;

namespace RecipeManager.UI.Blazor.Features.MealPlan.Models;

public class MealPlannerMonth
{
    public ICollection<MealPlannerWeek> Weeks { get; set; } = [];

    public void SetSupportingFields()
    {
        foreach (MealPlannerWeek week in Weeks)
        {
            week.SetDaysSupportingFields();
        }
    }

    public void AddRecipe(RecipeForMealPlanModel recipe)
    {
        foreach (MealPlannerWeek week in Weeks)
        {
            week.AddRecipe(recipe);
        }
    }

    public MealPlanForUpdateModel GetPlansToUpdate()
    {

        return new MealPlanForUpdateModel(Weeks.SelectMany(week => week.GetPlansToUpdate()).Select(plan => new MealPlanDayForUpdateModel
        (
            plan.Id == Guid.Empty ? null : plan.Id,
            plan.Date,
            plan.Recipes.Select(recipe => recipe.Id).ToList()
        )));
    }
}

using RecipeManager.UI.Blazor.Features.Recipes;

namespace RecipeManager.UI.Blazor.Features.MealPlan.Models;

public class MealPlannerDay
{
    public DateTimeOffset Date { get; set; }

    public bool IsSelected { get; set; }

    public ICollection<RecipeForDropdownModel> Recipes { get; set; } = [];


}

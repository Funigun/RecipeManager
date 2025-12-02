using RecipeManager.UI.Blazor.Features.Recipes;

namespace RecipeManager.UI.Blazor.Components.Pages.MealPlan;

public class MealPlannerDay
{
    public DateTimeOffset Date { get; set; }

    public bool IsSelected { get; set; }

    public ICollection<RecipeForDropdownModel> Recipes { get; set; } = [];
}

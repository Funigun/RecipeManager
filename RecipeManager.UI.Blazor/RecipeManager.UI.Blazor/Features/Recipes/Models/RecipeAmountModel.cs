using RecipeManager.UI.Blazor.Features.Units;

namespace RecipeManager.UI.Blazor.Features.Recipes.Models;

public sealed class RecipeAmountModel
{
    public double Amount { get; set; }

    public UnitForDropdownModel Unit { get; set; } = new();
}

using RecipeManager.UI.Blazor.Features.Ingredients;
using RecipeManager.UI.Blazor.Features.Units;

namespace RecipeManager.UI.Blazor.Features.Recipes.Models;

public sealed class RecipeIngredientModel
{
    public IngredientForDropdownModel Ingredient { get; set; }

    public UnitForDropdownModel Unit { get; set; }

    public double Amount { get; set; }

    public override bool Equals(object? obj)
    {
        return Ingredient.Id == ((RecipeIngredientModel?)obj)?.Ingredient.Id;
    }

    public override int GetHashCode()
    {
        return Ingredient.Id.GetHashCode();
    }
}

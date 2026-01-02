namespace RecipeManager.UI.Blazor.Features.Ingredients;

public sealed class IngredientForShoppingListModel
{
    public string Name { get; set; } = string.Empty;

    public double Quantity { get; set; }

    public string Unit { get; set; } = string.Empty;
}

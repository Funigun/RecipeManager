namespace RecipeManager.UI.Blazor.Features.Ingredients;

public class ShoppingListResponse
{
    public Dictionary<string, List<IngredientForShoppingListModel>> Ingredients { get; set; } = new();
}

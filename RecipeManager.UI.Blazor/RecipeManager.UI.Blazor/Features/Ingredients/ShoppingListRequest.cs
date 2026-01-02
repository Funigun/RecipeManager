using RecipeManager.UI.Blazor.Features.Recipes;

namespace RecipeManager.UI.Blazor.Features.Ingredients;

public class ShoppingListRequest
{
    public IEnumerable<RecipeForShoppingListModel> Recipes { get; set; } = [];
}

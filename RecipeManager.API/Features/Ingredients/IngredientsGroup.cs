namespace RecipeManager.Api.Features.Ingredients;

public static class IngredientsGroup
{
    public const string Route = "/api/ingredients";
    public const string Tag = "Ingredients";
    public const string IngredientByIdRoute = $"{Route}/{{ingredientId}}";
    public const string DropdownRoute = $"{Route}/dropdown";
    public const string GenerateShoppingListRoute = $"{Route}/generate-shopping-list";

    public static string IngredientById(Guid ingredientId) => $"{Route}/{ingredientId}";
}

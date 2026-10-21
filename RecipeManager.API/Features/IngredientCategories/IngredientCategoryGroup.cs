namespace RecipeManager.Api.Features.IngredientCategories;

public static class IngredientCategoryGroup
{
    public const string Route = "/api/ingredientcategories";
    public const string Tag = "Ingredient Categories";
    public const string CategoryByIdRoute = $"{Route}/{{categoryId}}";
    public const string DropdownRoute = $"{Route}/dropdown";

    public static string CategoryById(Guid categoryId) => $"{Route}/{categoryId}";
}

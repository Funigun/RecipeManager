namespace RecipeManager.Api.Features.RecipeCategories;

public static class RecipeCategoriesGroup
{
    public const string Route = "/api/recipecategories";
    public const string Tag = "Recipe Categories";
    public const string CategoryByIdRoute = $"{Route}/{{categoryId}}";
    public const string DropdownRoute = $"{Route}/dropdown";
    public const string FilteringRoute = $"{Route}/filtering";

    public static string CategoryById(Guid categoryId) => $"{Route}/{categoryId}";
}

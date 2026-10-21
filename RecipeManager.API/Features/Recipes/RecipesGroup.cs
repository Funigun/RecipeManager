namespace RecipeManager.Api.Features.Recipes;

public static class RecipesGroup
{
    public const string Route = "/api/recipes";
    public const string Tag = "Recipes";
    public const string RecipeByIdRoute = $"{Route}/{{id}}";
    public const string RecipeByRecipeIdRoute = $"{Route}/{{recipeId}}";
    public const string MealPlanRoute = $"{Route}/mealplan";
    public const string DropdownRoute = $"{Route}/dropdown";

    public static string RecipeById(Guid id) => $"{Route}/{id}";
    public static string RecipeByRecipeId(Guid recipeId) => $"{Route}/{recipeId}";
}

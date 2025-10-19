namespace RecipeManager.UI.Blazor.Features.RecipeCategories;

public static class RecipeCategoryTypeExtensions
{
    public static Dictionary<RecipeCategoryType, string> GetCategoryTypes()
    {
        return Enum.GetValues<RecipeCategoryType>().ToDictionary(group => group, group => group.ToFriendlyString());
    }

    private static string ToFriendlyString(this RecipeCategoryType categoryType)
    {
        return categoryType switch
        {
            RecipeCategoryType.Events => "Events",
            RecipeCategoryType.Cuisine => "Cuisine",
            RecipeCategoryType.Course => "Course",
            RecipeCategoryType.CookingMethod => "Cooking Method",
            _ => throw new ArgumentOutOfRangeException(nameof(categoryType), categoryType, null)
        };
    }
}

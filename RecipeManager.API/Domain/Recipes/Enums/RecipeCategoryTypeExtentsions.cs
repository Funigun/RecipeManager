namespace RecipeManager.Api.Domain.Recipes.Enums;

public static class RecipeCategoryTypeExtentsions
{
    public static bool IsRecipeCategoryType(this int type)
    {
        return Enum.IsDefined(typeof(RecipeCategoryType), type);
    }

    public static string ToFriendlyString(this RecipeCategoryType category)
    {
        return category switch
        {
            RecipeCategoryType.Events => "Events and occasions",
            RecipeCategoryType.Cuisine => "Cuisine",
            RecipeCategoryType.Course => "Course",
            RecipeCategoryType.MealType => "Meal Type",
            RecipeCategoryType.CookingMethod => "Cooking method",
            _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
        };
    }
}

using RecipeManager.UI.Blazor.Features.RecipeCategories.Models;

namespace RecipeManager.UI.Blazor.Features.RecipeCategories;

public static class RecipeCategoryTypeExtensions
{
    public static Dictionary<RecipeCategoryType, string> GetCategoryTypes()
    {
        return Enum.GetValues<RecipeCategoryType>().ToDictionary(group => group, group => group.ToFriendlyString());
    }

    public static string ToFriendlyString(this RecipeCategoryType categoryType)
    {
        return categoryType switch
        {
            RecipeCategoryType.Events => "Events",
            RecipeCategoryType.Cuisine => "Cuisine",
            RecipeCategoryType.Course => "Course",
            RecipeCategoryType.MealType => "Meal type",
            RecipeCategoryType.CookingMethod => "Cooking Method",
            _ => throw new ArgumentOutOfRangeException(nameof(categoryType), categoryType, null)
        };
    }

    public static Dictionary<RecipeCategoryType, IEnumerable<RecipeCategoryForDropdownModel>> GetCategoryTypesForRecipeFiltering()
    {
        return Enum.GetValues<RecipeCategoryType>()
                    .ToDictionary
                    (
                        group => group,
                        group => Enumerable.Empty<RecipeCategoryForDropdownModel>()
                    );
    }
}

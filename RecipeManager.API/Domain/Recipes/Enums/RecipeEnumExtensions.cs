namespace RecipeManager.Api.Domain.Recipes.Enums;

public static class RecipeEnumExtensions
{
    public static string GetName(this RecipeSectionType type)
    {
        return type switch
        {
            RecipeSectionType.PreCooking => "Pre Cooking",
            RecipeSectionType.IngredientsPreparation => "Ingredients Preparation",
            RecipeSectionType.Cooking => "Cooking Instructions",
            RecipeSectionType.Serving => "Serving",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public static bool IsRecipeSectionType(this int type)
    {
        return Enum.IsDefined(typeof(RecipeSectionType), type);
    }

    public static string ToFriendlyString(this RecipeDifficulty difficulty)
    {
        return difficulty switch
        {
            RecipeDifficulty.Easy => "Easy",
            RecipeDifficulty.Medium => "Medium",
            RecipeDifficulty.Advanced => "Advanced",
            RecipeDifficulty.Expert => "Expert",
            _ => throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null)
        };
    }

    public static bool IsRecipeDifficulty(this int difficulty)
    {
        return Enum.IsDefined(typeof(RecipeDifficulty), difficulty);
    }
}

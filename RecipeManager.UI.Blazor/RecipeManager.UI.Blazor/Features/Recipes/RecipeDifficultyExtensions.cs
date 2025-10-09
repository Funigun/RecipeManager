namespace RecipeManager.UI.Blazor.Features.Recipes;

public static class RecipeDifficultyExtensions
{
    public static string ToFriendlyString(this RecipeDifficulty group)
    {
        return group switch
        {
            RecipeDifficulty.Easy => "Easy",
            RecipeDifficulty.Medium => "Medium",
            RecipeDifficulty.Advanced => "Advanced",
            RecipeDifficulty.Expert => "Expert",
            _ => throw new ArgumentOutOfRangeException(nameof(group), group, null)
        };
    }

    public static Dictionary<RecipeDifficulty, string> GetRecipeDifficulties()
    {
        return Enum.GetValues<RecipeDifficulty>().ToDictionary(difficulty => difficulty, difficulty => difficulty.ToFriendlyString());
    }
}

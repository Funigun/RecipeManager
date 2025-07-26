using RecipeManager.Api.Domain.Recipes.Enums;

namespace RecipeManager.Api.Domain.Recipes.ValueObjects;

public sealed record RecipeSection
{
    public RecipeSectionType Type { get; set; }

    public ICollection<RecipeStep> Steps { get; set; } = [];

    private RecipeSection()
    {
    }

    public static RecipeSection Create(int type, IEnumerable<RecipeStep> steps)
    {
        return new()
        {
            Type = (RecipeSectionType)type,
            Steps = steps.ToList()
        };
    }
}

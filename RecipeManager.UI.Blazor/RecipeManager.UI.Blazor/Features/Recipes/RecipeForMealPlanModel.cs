using RecipeManager.UI.Blazor.Features.Ingredients;

namespace RecipeManager.UI.Blazor.Features.Recipes;

public sealed class RecipeForMealPlanModel
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public bool IsSelected { get; set; }

    public NutritionalValuesModel NutritionalValues { get; set; } = new();

    public RecipeForMealPlanModel CreateCopy()
    {
        return new()
        {
            Id = Id,
            Title = Title,
            ImageUrl = ImageUrl,
            NutritionalValues = NutritionalValues
        };
    }
}

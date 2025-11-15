namespace RecipeManager.UI.Blazor.Features.Ingredients;

public sealed class IngredientForDropdownModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? IngredientRecipe { get; set; }
}

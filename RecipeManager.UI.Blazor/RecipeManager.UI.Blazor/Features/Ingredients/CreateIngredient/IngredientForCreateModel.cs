namespace RecipeManager.UI.Blazor.Features.Ingredients.CreateIngredient;

public sealed class IngredientForCreateModel
{
    public string Name { get; set; } = string.Empty;

    public List<Guid> Categories { get; set; } = [];

    public List<Guid> Recipes { get; set; } = [];
}

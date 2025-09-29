namespace RecipeManager.UI.Blazor.Features.Ingredients.UpdateIngredient;

public sealed class IngredientForUpdateModel
{
    public string Name { get; set; } = string.Empty;

    public List<Guid> Categories { get; set; } = [];

    public List<Guid> Recipes { get; set; } = [];
}

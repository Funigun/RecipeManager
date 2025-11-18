using System.Collections.ObjectModel;

namespace RecipeManager.UI.Blazor.Features.Ingredients.UpdateIngredient;

public sealed class IngredientForUpdateModel
{
    public string Name { get; set; } = string.Empty;

    public Collection<Guid> Categories { get; set; } = [];

    public Collection<Guid> Recipes { get; set; } = [];
}

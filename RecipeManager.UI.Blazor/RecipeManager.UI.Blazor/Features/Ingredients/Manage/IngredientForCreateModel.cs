using System.Collections.ObjectModel;

namespace RecipeManager.UI.Blazor.Features.Ingredients.Manage;

public sealed class IngredientForCreateModel
{
    public string Name { get; set; } = string.Empty;

    public NutritionalValuesModel NutritionalValues { get; set; } = new();

    public Guid BaseUnit { get; set; }

    public IngredientPackageModel IngredientPackage { get; set; } = new();

    public IEnumerable<IngredientUnitConvertionModel> IngredientUnitConvertions { get; set; } = [];

    public Guid? ShoppingListCategoryId { get; set; }

    public Collection<Guid> Categories { get; set; } = [];

    public Collection<Guid> Recipes { get; set; } = [];
}

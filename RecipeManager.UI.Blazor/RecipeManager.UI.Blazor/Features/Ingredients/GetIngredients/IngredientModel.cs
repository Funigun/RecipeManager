namespace RecipeManager.UI.Blazor.Features.Ingredients.GetIngredients;

public sealed class IngredientModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string BaseUnit { get; set; } = string.Empty;

    public NutritionalValuesModel NutritionalValues { get; set; } = default!;

    public ICollection<IngredientUnitConvertionModel> IngredientUnitConvertions { get; set; } = [];
}

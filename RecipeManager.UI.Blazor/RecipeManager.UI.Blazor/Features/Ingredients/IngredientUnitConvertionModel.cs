namespace RecipeManager.UI.Blazor.Features.Ingredients;

public class IngredientUnitConvertionModel
{
    public Guid UnitToConvertId { get; set; }

    public string UnitToConvert { get; set; } = string.Empty;

    public double Ratio { get; set; }
}

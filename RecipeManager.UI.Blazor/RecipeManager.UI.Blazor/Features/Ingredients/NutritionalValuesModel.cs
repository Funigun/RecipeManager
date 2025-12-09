namespace RecipeManager.UI.Blazor.Features.Ingredients;

public sealed class NutritionalValuesModel
{
    public int Calories { get; set; }

    public double Proteins { get; set; }

    public double Fats { get; set; }

    public double Carbohydrates { get; set; }

    public int IngredientAmount { get; set; }

    public Guid IngredientUnitId { get; set; }
}

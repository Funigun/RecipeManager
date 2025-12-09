using RecipeManager.Api.Domain.Units;

namespace RecipeManager.Api.Domain.Common;

public sealed class NutritionalValue
{
    public int Calories { get; set; }

    public double Carbohydrates { get; set; }

    public double Fats { get; set; }

    public double Proteins { get; set; }

    public int IngredientAmount { get; set; }

    public UnitId IngredientUnit { get; set; } = default!;
}

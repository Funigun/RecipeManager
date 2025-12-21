using RecipeManager.Api.Domain.Units;

namespace RecipeManager.Api.Domain.Ingredients;

public sealed class IngredientUnitConvertion
{
    public UnitId UnitToConvertId { get; set; } = default!;

    public double Ratio { get; set; }
}

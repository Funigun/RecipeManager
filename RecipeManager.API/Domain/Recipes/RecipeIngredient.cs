using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Units;

namespace RecipeManager.Api.Domain.Recipes;

public sealed class RecipeIngredient
{
    public int Id { get; set; }

    public RecipeId RecipeId { get; set; } = default!;

    public IngredientId IngredientId { get; set; } = default!;

    public UnitId UnitId { get; set; } = default!;

    public double Amount { get; set; }

    private RecipeIngredient()
    {
    }

    public static RecipeIngredient Create(IngredientId ingredientId, UnitId unitId, double amount)
    {
        return new()
        {
            IngredientId = ingredientId,
            UnitId = unitId,
            Amount = amount
        };
    }
}

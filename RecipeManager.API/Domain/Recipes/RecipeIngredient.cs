using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Units;

namespace RecipeManager.Api.Domain.Recipes;

public sealed class RecipeIngredient
{
    public int Id { get; set; }

    public RecipeId RecipeId { get; set; } = default!;

    public IngredientId IngredientId { get; set; } = default!;

    public UnitId UnitId { get; set; } = default!;

    public decimal Amount { get; set; }

    private RecipeIngredient()
    {
    }

    public static RecipeIngredient Create(RecipeId recipeId, IngredientId ingredientId, UnitId unitId, decimal amount)
    {
        return new()
        {
            RecipeId = recipeId,
            IngredientId = ingredientId,
            UnitId = unitId,
            Amount = amount
        };
    }
}

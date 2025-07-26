using RecipeManager.Api.Domain.Units;

namespace RecipeManager.Api.Domain.Recipes.ValueObjects;

public sealed record RecipeAmount
{
    public double Amount { get; init; }

    public UnitId UnitId { get; init; } = default!;

    public Unit Unit { get; init; } = default!;

    public RecipeAmount(double amount, UnitId unitId)
    {
        Amount = amount;
        UnitId = unitId;
    }

    private RecipeAmount()
    {
    }

    public static RecipeAmount Create(double amount, UnitId unitId)
    {
        return amount <= 0
             ? throw new ArgumentException("Recipe amount must be greater than 0.")
             : new RecipeAmount(amount, unitId);
    }
}

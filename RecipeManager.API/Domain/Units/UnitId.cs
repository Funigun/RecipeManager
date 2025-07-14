namespace RecipeManager.Api.Domain.Units;

public record struct UnitId(Guid Value)
{
    public override readonly string ToString() => Value.ToString();
}

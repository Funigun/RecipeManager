namespace RecipeManager.API.Domain.Units;

public record struct UnitId(Guid Value)
{
    public static implicit operator Guid(UnitId id) => id.Value;

    public static implicit operator UnitId(Guid id) => new(id);

    public override string ToString() => Value.ToString();
}

namespace RecipeManager.Api.Domain.Units;

public record UnitId(Guid Value)
{
    public static implicit operator Guid(UnitId id) => id.Value;

    public static implicit operator UnitId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}

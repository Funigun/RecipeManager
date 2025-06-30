namespace RecipeManager.API.Domain.Units;

public record struct UnitId(Guid Value)
{
    public static Guid ToGuid(UnitId id) => id.Value;

    public static implicit operator Guid(UnitId id) => id.Value;

    public static UnitId FromGuid(Guid id) => new(id);

    public static implicit operator UnitId(Guid id) => new(id);

    public override readonly string ToString() => Value.ToString();
}

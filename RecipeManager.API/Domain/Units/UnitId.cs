using RecipeManager.Api.Domain.Common.Abstractions;

namespace RecipeManager.Api.Domain.Units;

public record UnitId(Guid Value) : IEntityId
{
    public static implicit operator Guid(UnitId id) => id.Value;

    public static implicit operator UnitId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}

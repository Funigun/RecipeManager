using RecipeManager.Api.Domain.Common.Abstractions;
using RecipeManager.Api.Domain.Units.Enums;

namespace RecipeManager.Api.Domain.Units;

public sealed class Unit : AuditableEntity, IEntity<UnitId>
{
    public UnitId Id { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string? ShortName { get; set; }

    public UnitGroup Group { get; set; }

    private Unit()
    {
    }

    public static Unit Create(string name, string? shortName, UnitGroup group)
    {
        return new()
        {
            Name = name,
            ShortName = shortName,
            Group = group,
        };
    }

    public void Update(string name, string? shortName, UnitGroup group)
    {
        Name = name;
        ShortName = shortName;
        Group = group;
    }
}

using RecipeManager.API.Domain.Common.Abstractions;
using RecipeManager.API.Domain.Units.Enums;

namespace RecipeManager.API.Domain.Units;

public sealed class Unit : AuditableEntity, IEntity<UnitId>
{
    public UnitId Id { get; set; }

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

using RecipeManager.Api.Domain.Common.Abstractions;
using RecipeManager.Api.Domain.Units.Enums;

namespace RecipeManager.Api.Domain.Units;

public sealed class Unit : AuditableEntity, IEntity<UnitId>
{
    public UnitId Id { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string? ShortName { get; set; }

    public string PluralName { get; set; }

    public string? PluralShortName { get; set; }

    public UnitGroup Group { get; set; }

    public bool IsBaseUnit { get; set; }

    public UnitId? PrimaryUnit { get; set; }

    public int ConversionFactor { get; set; }

    private Unit()
    {
    }

    public static Unit Create(string name, string? shortName, string pluralName, string? pluralShortName, UnitGroup group, bool isBaseUnit, UnitId? primaryUnitId, int conversionFactory)
    {
        return new()
        {
            Name = name,
            ShortName = shortName,
            PluralName = pluralName,
            PluralShortName = pluralShortName,
            Group = group,
            IsBaseUnit = isBaseUnit,
            PrimaryUnit = primaryUnitId,
            ConversionFactor = conversionFactory
        };
    }

    public void Update(string name, string? shortName, string pluralName, string? pluralShortName, UnitGroup group, bool isBaseUnit, UnitId? primaryUnitId, int conversionFactory)
    {
        Name = name;
        ShortName = shortName;
        PluralName = pluralName;
        PluralShortName = pluralShortName;
        Group = group;
        IsBaseUnit = isBaseUnit;
        PrimaryUnit = primaryUnitId;
        ConversionFactor = conversionFactory;
    }
}

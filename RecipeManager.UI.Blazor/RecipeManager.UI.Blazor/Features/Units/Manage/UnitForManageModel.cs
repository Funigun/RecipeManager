namespace RecipeManager.UI.Blazor.Features.Units.Manage;

public sealed class UnitForManageModel
{
    public string Name { get; set; } = string.Empty;

    public string? ShortName { get; set; }

    public string PluralName { get; set; } = string.Empty;

    public string? PluralShortName { get; set; }

    public UnitGroup Group { get; set; }

    public bool IsBaseUnit { get; set; }

    public Guid? PrimaryUnit { get; set; }

    public int ConversionFactor { get; set; }
}

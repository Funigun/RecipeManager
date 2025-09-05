namespace RecipeManager.UI.Blazor.Features.Units.UpdateUnit;

public sealed class UnitForUpdateModel
{
    public Guid UnitId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? ShortName { get; set; }

    public UnitGroup Group { get; set; }

    public Guid? PrimaryUnit { get; set; }

    public int ConversionFactor { get; set; }
}

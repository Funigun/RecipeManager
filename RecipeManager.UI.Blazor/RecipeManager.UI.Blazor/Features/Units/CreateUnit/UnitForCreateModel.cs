namespace RecipeManager.UI.Blazor.Features.Units.CreateUnit;

public sealed class UnitForCreateModel
{
    public string Name { get; set; } = string.Empty;

    public string? ShortName { get; set; }

    public UnitGroup Group { get; set; }

    public Guid? PrimaryUnit { get; set; }

    public int ConversionFactor { get; set; }
}

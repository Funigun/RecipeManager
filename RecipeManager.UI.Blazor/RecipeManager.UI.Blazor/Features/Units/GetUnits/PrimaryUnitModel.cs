namespace RecipeManager.UI.Blazor.Features.Units.GetUnits;

public sealed class PrimaryUnitModel
{
    public Guid UnitId { get; set; } = Guid.Empty;

    public string Name { get; set; } = string.Empty;

    public int ConversionFactory { get; set; }
}

namespace RecipeManager.UI.Blazor.Features.Units.GetUnits;

public sealed class UnitModel
{
    public Guid UnitId { get; set; } = Guid.Empty;

    public string Name { get; set; } = string.Empty;

    public string PluralName { get; set; } = string.Empty;

    public string? ShortName { get; set; }

    public string? PluralShortName { get; set; }

    public string Group { get; set; } = string.Empty;

    public PrimaryUnitModel? PrimaryUnit { get; set; }
}

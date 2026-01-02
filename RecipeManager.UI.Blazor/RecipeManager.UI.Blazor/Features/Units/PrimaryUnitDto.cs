namespace RecipeManager.UI.Blazor.Features.Units;

public sealed record PrimaryUnitDto
{
    public Guid? UnitId { get; set; }

    public string Name { get; set; } = string.Empty;
}

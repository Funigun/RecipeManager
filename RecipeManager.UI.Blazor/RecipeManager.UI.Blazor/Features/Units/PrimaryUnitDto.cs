namespace RecipeManager.UI.Blazor.Features.Units;

public sealed record PrimaryUnitDto
{
    public Guid? UnitId { get; init; }

    public string Name { get; init; }
}

using RecipeManager.Api.Domain.Units;

namespace RecipeManager.Api.Domain.Ingredients;

public sealed class IngredientPackage
{
    public int Id { get; set; }

    public IngredientId IngredientId { get; set; } = default!;

    public UnitId PackageUnitId { get; set; } = default!;

    public int PackageSize { get; set; }

    public UnitId PackageSizeUnitId { get; set; } = default!;
}

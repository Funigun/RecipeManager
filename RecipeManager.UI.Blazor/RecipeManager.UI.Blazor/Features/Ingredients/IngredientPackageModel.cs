namespace RecipeManager.UI.Blazor.Features.Ingredients;

public sealed class IngredientPackageModel
{
    public Guid PackageUnitId { get; set; }

    public string PackageUnit { get; set; } = string.Empty;

    public int PackageSize { get; set; }

    public Guid PackageSizeUnitId { get; set; }

    public string PackageSizeUnit { get; set; } = string.Empty;
}

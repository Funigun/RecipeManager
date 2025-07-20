using RecipeManager.Api.Domain.Common.Abstractions;

namespace RecipeManager.Api.Domain.Ingredients;

public sealed class IngredientCategory : AuditableEntity, IEntity<IngredientCategoryId>
{
    private List<IngredientCategory> _subcategories = [];

    public IngredientCategoryId Id { get; set; } = default!;

    public string Name { get; set; } = string.Empty;

    public IngredientCategoryId? ParentId { get; set; }

    public IReadOnlyList<IngredientCategory> Subcategories => _subcategories.ToList();

    private IngredientCategory()
    {
    }

    public static IngredientCategory Create(string name, IEnumerable<IngredientCategory> subcategories)
    {
        return new()
        {
            Name = name,
            _subcategories = subcategories.Select(subcategory => IngredientCategory.Create(subcategory.Name, subcategory.Subcategories))
                                          .ToList(),
        };
    }
}

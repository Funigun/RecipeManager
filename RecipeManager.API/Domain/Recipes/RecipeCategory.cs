using RecipeManager.Api.Domain.Common.Abstractions;

namespace RecipeManager.Api.Domain.Recipes;

public sealed class RecipeCategory : AuditableEntity, IEntity<RecipeCategoryId>
{
    private List<RecipeCategory> _subcategories = [];

    public RecipeCategoryId Id { get; set; } = default!;

    public string Name { get; set; } = string.Empty;

    public RecipeCategoryId? ParentId { get; set; }

    public IReadOnlyList<RecipeCategory> Subcategories => _subcategories.ToList();

    private RecipeCategory()
    {
    }

    public static RecipeCategory Create(string name, IEnumerable<RecipeCategory> subcategories)
    {
        return new()
        {
            Name = name,
            _subcategories = subcategories.Select(subcategory => RecipeCategory.Create(subcategory.Name, subcategory.Subcategories))
                                          .ToList(),
        };
    }
}

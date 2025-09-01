using RecipeManager.Api.Domain.Common.Abstractions;
using RecipeManager.Api.Domain.Recipes;

namespace RecipeManager.Api.Domain.Cookbooks;

public sealed class CookbookCategory : AuditableEntity, IEntity<CookbookCategoryId>
{
    private List<CookbookCategory> _subcategories = [];

    private List<RecipeId> _recipes = [];

    public CookbookCategoryId Id { get; set; } = default!;

    public string Name { get; set; } = string.Empty;

    public Cookbook Cookbook { get; set; } = default!;

    public CookbookCategoryId? ParentId { get; set; }

    public IReadOnlyList<CookbookCategory> Subcategories => _subcategories.ToList();

    public IReadOnlyList<RecipeId> Recipes => _recipes.ToList();

    private CookbookCategory()
    {
    }

    public static CookbookCategory Create(string name, Cookbook cookbook, IEnumerable<CookbookCategory> subcategories, IEnumerable<RecipeId> recipes)
    {
        return new()
        {
            Name = name,
            Cookbook = cookbook,
            _subcategories = subcategories.ToList(),
            _recipes = recipes.ToList()
        };
    }
}

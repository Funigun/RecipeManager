using RecipeManager.Api.Domain.Common.Abstractions;

namespace RecipeManager.Api.Domain.Cookbooks;

public sealed class Cookbook : AuditableEntity, IEntity<CookbookId>
{
    private List<CookbookCategory> _categories = [];

    public CookbookId Id { get; set; } = default!;

    public string Title { get; set; } = string.Empty;

    public IReadOnlyList<CookbookCategory> Categories => _categories.ToList();

    private Cookbook()
    {
    }

    public static Cookbook Create(string title, IEnumerable<CookbookCategory> categories)
    {
        return new()
        {
            Title = title,
            _categories = categories.ToList()
        };
    }

    public void Update(string title, IEnumerable<CookbookCategory> categories)
    {
        Title = title;
        _categories = categories.ToList();
    }
}

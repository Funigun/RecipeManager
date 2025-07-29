using RecipeManager.Api.Domain.Common.Abstractions;

namespace RecipeManager.Api.Domain.Recipes;

public sealed class RecipeCategory : AuditableEntity, IEntity<RecipeCategoryId>
{
    public RecipeCategoryId Id { get; set; } = default!;

    public string Name { get; set; } = string.Empty;

    private RecipeCategory()
    {
    }

    public static RecipeCategory Create(string name)
    {
        return new()
        {
            Name = name,
        };
    }
}

using RecipeManager.Api.Domain.Common.Abstractions;
using RecipeManager.Api.Domain.Recipes.Enums;

namespace RecipeManager.Api.Domain.Recipes;

public sealed class RecipeCategory : AuditableEntity, IEntity<RecipeCategoryId>
{
    public RecipeCategoryId Id { get; set; } = default!;

    public string Name { get; set; } = string.Empty;

    public RecipeCategoryType Type { get; set; }

    private RecipeCategory()
    {
    }

    public static RecipeCategory Create(string name, RecipeCategoryType type)
    {
        return new()
        {
            Name = name,
            Type = type
        };
    }
}

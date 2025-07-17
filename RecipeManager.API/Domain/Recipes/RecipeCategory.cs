using RecipeManager.Api.Domain.Common.Abstractions;

namespace RecipeManager.Api.Domain.Recipes;

public sealed class RecipeCategory : AuditableEntity, IEntity<RecipeCategoryId>
{
    public RecipeCategoryId Id { get; set; }

    public string Name { get; set; } = string.Empty;
}

using RecipeManager.Api.Domain.Common.Abstractions;

namespace RecipeManager.Api.Domain.Recipes;

public sealed class Recipe : AuditableEntity, IEntity<RecipeId>
{
    public RecipeId Id { get; set; }

    public string Name { get; set; } = string.Empty;
}

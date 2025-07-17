using RecipeManager.Api.Domain.Common.Abstractions;

namespace RecipeManager.Api.Domain.Cookbooks;

public sealed class Cookbook : AuditableEntity, IEntity<CookbookId>
{
    public CookbookId Id { get; set; }

    public string Name { get; set; } = string.Empty;
}

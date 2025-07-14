using RecipeManager.Shared.Contracts.Authorization;

namespace RecipeManager.Identity.Api.Domain;

public sealed class Permission
{
    public UserPermission Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}

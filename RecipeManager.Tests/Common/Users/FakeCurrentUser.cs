using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Integration.Tests.Common.Users;

public class FakeCurrentUser : ICurrentUser
{
    public virtual string Id { get; set; } = default!;

    public virtual IEnumerable<string> Roles { get; set; } = [];

    public FakeCurrentUser()
    {
    }

    public FakeCurrentUser(string id, IEnumerable<string> roles)
    {
        Id = id;
        Roles = roles ?? [];
    }

    public bool HasRole(string role)
    {
        return Roles.Contains(role);
    }
}

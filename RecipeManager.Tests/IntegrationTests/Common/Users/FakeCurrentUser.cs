using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Tests.IntegrationTests.Common.Users;

public class FakeCurrentUser : ICurrentUser
{
    public virtual int Id { get; set; }

    public virtual IEnumerable<string> Roles { get; set; } = [];

    public FakeCurrentUser()
    {
    }

    public FakeCurrentUser(int id, IEnumerable<string> roles)
    {
        Id = id;
        Roles = roles ?? [];
    }

    public bool HasRole(string role)
    {
        return Roles.Contains(role);
    }
}

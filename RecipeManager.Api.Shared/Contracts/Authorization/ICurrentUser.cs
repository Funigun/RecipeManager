namespace RecipeManager.Api.Shared.Contracts.Authorization;

public interface ICurrentUser
{
    string Id { get; set; }

    IEnumerable<string> Roles { get; set; }

    bool HasRole(string role);
}

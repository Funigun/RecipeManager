namespace RecipeManager.Api.Shared.Contracts.Authorization;

public interface ICurrentUser
{
    int Id { get; init; }

    bool HasRole(string role);
}

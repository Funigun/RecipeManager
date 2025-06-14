namespace RecipeManager.Shared.Contracts.User.Login;

public sealed record UserLoginDto
{
    public string UserName { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;
}
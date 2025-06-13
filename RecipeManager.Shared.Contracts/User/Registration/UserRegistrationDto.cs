namespace RecipeManager.Shared.Contracts.User.Registration;

public sealed record UserRegistrationDto
{
    public string UserName { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;
}
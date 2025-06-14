namespace RecipeManager.Shared.Contracts.User.Registration;

public sealed class UserRegistrationModel
{
    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string ConfirmationPassword { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}
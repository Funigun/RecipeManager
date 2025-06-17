namespace RecipeManager.UI.Blazor.Features.Account.Register;

public sealed class RegistrationRequest
{
    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string ConfirmationPassword { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}
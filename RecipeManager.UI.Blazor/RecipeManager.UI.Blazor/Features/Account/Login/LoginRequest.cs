namespace RecipeManager.UI.Blazor.Features.Account.Login;

public sealed class LoginRequest
{
    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}

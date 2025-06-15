namespace RecipeManager.UI.Blazor.Components.Pages.Account;

public sealed class LoginRequest
{
    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}

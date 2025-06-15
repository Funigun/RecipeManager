namespace RecipeManager.UI.Blazor.Components.Pages.Account;

public sealed class LoginResponse
{
    public string Token { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public DateTime ExpirationDate { get; set; }
}

namespace RecipeManager.Shared.Contracts.User.Login;

public sealed class UserLoginModel
{
    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
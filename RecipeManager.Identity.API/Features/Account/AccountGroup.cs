namespace RecipeManager.Identity.Api.Features.Account;

public static class AccountGroup
{
    public const string Route = "/api/account";
    public const string Tag = "account";
    public const string RegisterRoute = $"{Route}/register";
    public const string ChangePasswordRoute = $"{Route}/change-password/{{userId}}";

    public static string ChangePassword(Guid userId) => $"{Route}/change-password/{userId}";
}

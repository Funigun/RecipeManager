namespace RecipeManager.Shared.Contracts.Authorization;

public static class UserRole
{
    public const string Admin = "Admin";

    public const string User = "User";

    public const string Guest = "Guest";

    public static readonly IEnumerable<string> AllRoles = [ Admin, User, Guest ];
}

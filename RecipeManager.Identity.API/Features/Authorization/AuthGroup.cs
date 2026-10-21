namespace RecipeManager.Identity.Api.Features.Authorization;

public static class AuthGroup
{
    public const string Route = "/api/auth";
    public const string Tag = "auth";
    public const string LogInRoute = $"{Route}/login";
    public const string LogOutRoute = $"{Route}/logout";
    public const string RefreshTokenRoute = $"{Route}/refresh-token";
}

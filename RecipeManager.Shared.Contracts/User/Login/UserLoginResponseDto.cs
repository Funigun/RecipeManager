namespace RecipeManager.Shared.Contracts.User.Login;

public sealed record UserLoginResponseDto
{
    public string Token { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}

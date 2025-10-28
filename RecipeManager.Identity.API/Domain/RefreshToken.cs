namespace RecipeManager.Identity.Api.Domain;

public sealed class RefreshToken
{
    public int Id { get; set; }

    public string Token { get; set; } = string.Empty;

    public DateTime ExpirationDate { get; set; }

    public User User { get; set; } = default!;
}

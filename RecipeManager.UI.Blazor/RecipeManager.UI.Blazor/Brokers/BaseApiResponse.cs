namespace RecipeManager.UI.Blazor.Brokers;

public class BaseApiResponse
{
    public int StatusCode { get; init; }

    public string Message { get; init; } = string.Empty;

    public IEnumerable<string> Errors { get; init; } = [];
}

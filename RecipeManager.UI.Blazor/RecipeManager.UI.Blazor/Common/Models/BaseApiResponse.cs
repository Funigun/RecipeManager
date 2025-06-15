namespace RecipeManager.UI.Blazor.Common.Models;

public class BaseApiResponse
{
    public int StatusCode { get; init; }

    public string Message { get; init; } = string.Empty;

    public IEnumerable<string> Errors { get; init; } = [];
}

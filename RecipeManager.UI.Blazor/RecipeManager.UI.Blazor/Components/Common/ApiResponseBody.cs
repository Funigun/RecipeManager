namespace RecipeManager.UI.Blazor.Components.Common;

public class ApiResponseBody
{
    public int StatusCode { get; init; }

    public string Message { get; init; } = string.Empty;

    public IEnumerable<string> Errors { get; set; } = [];

    public Dictionary<string, IEnumerable<string>> ValidationErrors { get; init; } = [];
}

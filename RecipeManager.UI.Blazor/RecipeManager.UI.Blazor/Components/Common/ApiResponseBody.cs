namespace RecipeManager.UI.Blazor.Components.Common;

public class ApiResponseBody
{
    public int StatusCode { get; set; }
    
    public string Message { get; set; } = string.Empty;

    public IEnumerable<string> Errors { get; set; } = [];

    public Dictionary<string, IEnumerable<string>> ValidationErrors { get; set; } = [];
}

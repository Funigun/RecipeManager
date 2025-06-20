namespace RecipeManager.Api.Shared.Contracts.Exceptions;

public sealed record ResponseBody
{
    required public int StatusCode { get; init; }

    required public string Message { get; init; }

    public IEnumerable<string> Errors { get; init; } = [];

    public Dictionary<string, IEnumerable<string>> ValidationErrors { get; init; } = [];
}

namespace RecipeManager.Api.Shared.Contracts.Exceptions;

public sealed record ResponseBody
{
    public required int StatusCode { get; init; }

    public required string Message { get; init; }

    public object? Errors { get; init; }
}

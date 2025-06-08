namespace RecipeManager.Api.Shared.Contracts.Exceptions;

internal sealed record ResponseBody
{
    internal required int StatusCode { get; init; }

    internal required string Message { get; init; }

    internal object? Errors { get; init; }
}

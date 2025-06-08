namespace RecipeManager.Api.Shared.Contracts.Exceptions;

public abstract class ApplicationValidationException : Exception
{
    public IEnumerable<string> Errors { get; } = [];
}

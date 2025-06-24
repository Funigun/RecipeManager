namespace RecipeManager.Api.Shared.Contracts.Exceptions;

public abstract class ApplicationValidationException : Exception
{
    protected ApplicationValidationException(string message) : base(message)
    {
    }

    public IEnumerable<string> Errors { get; protected set; } = [];

    public Dictionary<string, IEnumerable<string>> ValidationErrors { get; protected set; } = [];
}

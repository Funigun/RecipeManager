namespace RecipeManager.Api.Application;

public readonly union Result<T>(T, ValidationError, AuthorizationError, NotFoundError);

public sealed record ValidationError(IReadOnlyDictionary<string, string[]> Errors);

public sealed record AuthorizationError;

public sealed record NotFoundError;

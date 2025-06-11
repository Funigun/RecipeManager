using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Shared.Filters;

internal class AuthorizationFilter<TRequest>(IAuthorizationPolicy<TRequest> authorizationPolicy) : BaseEnpointFilter
{
    protected override async ValueTask<object?> OnBeforeExecutionAsync(EndpointFilterInvocationContext context)
    {
        object? request = context.Arguments.FirstOrDefault(a => a?.GetType() == typeof(TRequest));

        if (request == null)
        {
            List<ValidationFailure> error = [new("", "Invalid request format")];
            throw new ValidationException(error);
        }

        if (!await authorizationPolicy.IsAuthorized((TRequest)request!))
        {
            throw new UnauthorizedAccessException("You are not authorized to perform this action.");
        }

        return null;
    }
}

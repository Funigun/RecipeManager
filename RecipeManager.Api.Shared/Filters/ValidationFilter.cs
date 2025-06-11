using Microsoft.AspNetCore.Http;
using FluentValidation;
using FluentValidation.Results;
using RecipeManager.Api.Shared.Contracts.Exceptions;

namespace RecipeManager.Api.Shared.Filters;

public class ValidationFilter<TRequest>(IValidator<TRequest> validator) : BaseEnpointFilter
{
    protected override async ValueTask<object?> OnBeforeExecutionAsync(EndpointFilterInvocationContext context)
    {
        object? request = context.Arguments.FirstOrDefault(a => a?.GetType() == typeof(TRequest));

        if (request == null)
        {
            List<ValidationFailure> error = [new("", "Invalid request format")];
            throw new ValidationException(error);
        }

        ValidationContext<TRequest>? validationContext = new((TRequest)request);
        ValidationResult validationResult = await validator.ValidateAsync(validationContext);

        if (!validationResult.IsValid)
        {
            throw CustomValidationException.ValidationFailed
            (
                validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                                       .ToList()
            );
        }

        return null;
    }
}

public class CustomValidationException : ApplicationValidationException
{
    private CustomValidationException(string message) : base(message)
    {
    }

    public static CustomValidationException ValidationFailed(List<string> errors)
    {
        CustomValidationException exception = new("Registration failed. Please check the provided data and try again.")
        {
            Errors = errors
        };

        return exception;
    }
}

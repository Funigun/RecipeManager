using Microsoft.AspNetCore.Http;
using FluentValidation;
using FluentValidation.Results;

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
            throw new ValidationException(validationResult.Errors);
        }

        return null;
    }
}

using FluentValidation;
using FluentValidation.Results;
using RecipeManager.Api.Application;

namespace RecipeManager.Api.Features.Validation;

public abstract class BaseValidator<TRequest> : AbstractValidator<TRequest>
{
    public static ValidationError ToValidationError(ValidationResult validationResult)
    {
        ArgumentNullException.ThrowIfNull(validationResult);

        Dictionary<string, string[]> errors = validationResult.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray());

        return new ValidationError(errors);
    }
}

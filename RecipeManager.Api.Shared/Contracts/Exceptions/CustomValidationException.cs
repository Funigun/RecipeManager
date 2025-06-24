using FluentValidation.Results;

namespace RecipeManager.Api.Shared.Contracts.Exceptions;

public class CustomValidationException : ApplicationValidationException
{
    private CustomValidationException(string message) : base(message)
    {
    }

    public static CustomValidationException ValidationFailed(IEnumerable<ValidationFailure> errors)
    {
        CustomValidationException exception = new("Registration failed. Please check the provided data and try again.")
        {
            ValidationErrors = errors.GroupBy(error => error.PropertyName)
                                     .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage)),
        };

        return exception;
    }
}

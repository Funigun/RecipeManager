using RecipeManager.Api.Shared.Contracts.Exceptions;

namespace RecipeManager.Identity.API.Common.Exceptions;

public class IdentityValidationException : ApplicationValidationException
{
    private IdentityValidationException(string message) : base(message)
    {
    }

    public static IdentityValidationException RegistrationFailed(IEnumerable<string> errors)
    {
        IdentityValidationException exception = new("Registration failed. Please check the provided data and try again.")
        {
            Errors = errors
        };

        return exception;
    }

    public static IdentityValidationException UserNameAlreadyInUse() => new("Registration failed")
    {
        Errors = ["User name already in use"]
    };
}

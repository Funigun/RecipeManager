using FluentValidation;
using RecipeManager.Shared.Contracts.User.Registration;

namespace RecipeManager.UI.Blazor.Features.Account.Register;

public sealed class RegistrationValidator : AbstractValidator<RegistrationRequest>
{
    public RegistrationValidator()
    {

        RuleFor(x => x.UserName).SetValidator(new UserNameValidator());
        RuleFor(x => x.Email).SetValidator(new EmailValidator());
        RuleFor(x => x.Password).SetValidator(new PasswordValidator());
        RuleFor(x => x.ConfirmationPassword).Equal(x => x.Password)
            .WithMessage("Confirmation password must match the password.");
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<RegistrationRequest>.CreateWithOptions((RegistrationRequest)model, x => x.IncludeProperties(propertyName)));

        return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
    };
}
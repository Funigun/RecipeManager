using FluentValidation;
using RecipeManager.Shared.Contracts.User.Registration;
using RecipeManager.UI.Blazor.Components.Common;

namespace RecipeManager.UI.Blazor.Features.Account.Register;

public sealed class RegistrationValidator : BaseAbstractValidator<RegistrationRequest>
{
    public RegistrationValidator()
    {
        RuleFor(x => x.UserName).SetValidator(new UserNameValidator());
        RuleFor(x => x.Email).SetValidator(new EmailValidator());
        RuleFor(x => x.Password).SetValidator(new PasswordValidator());
        RuleFor(x => x.ConfirmationPassword).Equal(x => x.Password)
            .WithMessage("Confirmation password must match the password.");
    }
}

using FluentValidation;

namespace RecipeManager.UI.Blazor.Contracts.User.Registration;

public sealed class EmailValidator : AbstractValidator<string>
{
    public EmailValidator()
    {
        RuleFor(email => email)
            .NotEmpty()
                .WithMessage("Email is required")
            .EmailAddress()
                .WithMessage("Invalid email format");
    }
}

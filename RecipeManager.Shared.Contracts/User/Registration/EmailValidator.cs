using FluentValidation;

namespace RecipeManager.Shared.Contracts.User.Registration;

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

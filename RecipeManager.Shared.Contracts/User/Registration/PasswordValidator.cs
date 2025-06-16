using FluentValidation;

namespace RecipeManager.Shared.Contracts.User.Registration;

public sealed class PasswordValidator : AbstractValidator<string>
{
    public PasswordValidator()
    {
            RuleFor(password => password)
            .NotEmpty()
                .WithMessage("Password is required")
            .MinimumLength(10)
                .WithMessage("Password must be at least 10 characters long")
            .Matches(@"[a-z]")
                .WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"[A-Z]")
                .WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[0-9]")
                .WithMessage("Password must contain at least one digit")
            .Matches(@"[^a-zA-Z0-9]")
                .WithMessage("Password must contain at least one special character");
    }
}

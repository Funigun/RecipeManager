using FluentValidation;

namespace RecipeManager.Shared.Contracts.User.Registration;

public sealed class UserNameValidator : AbstractValidator<string>
{
    public UserNameValidator()
    {
        RuleFor(userName => userName)
            .NotEmpty()
                .WithMessage("Username is required")
            .MaximumLength(15)
                .WithMessage("Username must not exceed 15 characters");
    }
}

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

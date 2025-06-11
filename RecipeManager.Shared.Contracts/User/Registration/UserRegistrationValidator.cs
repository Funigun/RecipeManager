using FluentValidation;

namespace RecipeManager.Shared.Contracts.User.Registration;

public sealed class UserRegistrationValidator : AbstractValidator<UserRegistrationDto>
{
    public UserRegistrationValidator()
    {
        RuleFor(dto => dto.UserName)
            .NotEmpty()
                .WithMessage("Username is required")
            .MaximumLength(15)
                .WithMessage("Username must not exceed 15 characters");

        RuleFor(dto => dto.Password)
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


        RuleFor(dto => dto.Email)
            .NotEmpty()
                .WithMessage("Email is required")
            .EmailAddress()
                .WithMessage("Invalid email format");
    }
}

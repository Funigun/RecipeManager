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

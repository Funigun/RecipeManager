using FluentValidation;

namespace RecipeManager.Shared.Contracts.Cookbooks;

public sealed class CookbookTitleValidator : AbstractValidator<string>
{
    public CookbookTitleValidator()
    {
        RuleFor(title => title)
            .NotEmpty()
                .WithMessage("Cookbook title must not be empty.")
            .MaximumLength(100)
                .WithMessage("Cookbook title must not exceed 100 characters.");
    }
}

using FluentValidation;

namespace RecipeManager.Shared.Contracts.Cookbooks;

public sealed class CookbookCategoryNameValidator : AbstractValidator<string>
{
    public CookbookCategoryNameValidator()
    {
        RuleFor(name => name)
            .NotEmpty()
                .WithMessage("Category name must not be empty.")
            .MaximumLength(100)
                .WithMessage("Category name must not exceed 100 characters.");
    }
}

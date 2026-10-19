using FluentValidation;

namespace RecipeManager.Api.Contracts.RecipeCategories;

public sealed class RecipeCategoryNameValidator : AbstractValidator<string>
{
    private const int MaxLength = 100;

    public RecipeCategoryNameValidator()
    {
        RuleFor(x => x)
            .NotEmpty()
                .WithMessage("Category name cannot be empty")
            .MaximumLength(MaxLength)
                .WithMessage($"Category name cannot exceed {MaxLength} characters");
    }
}

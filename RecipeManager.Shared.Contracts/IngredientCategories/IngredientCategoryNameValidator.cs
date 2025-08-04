using FluentValidation;

namespace RecipeManager.Shared.Contracts.IngredientCategories;

public sealed class IngredientCategoryNameValidator : AbstractValidator<string>
{
    private const int MaxLength = 100;

    public IngredientCategoryNameValidator()
    {
        RuleFor(x => x)
            .NotEmpty()
                .WithMessage("Category name cannot be empty")
            .MaximumLength(MaxLength)
                .WithMessage($"Category name cannot exceed {MaxLength} characters");
    }
}

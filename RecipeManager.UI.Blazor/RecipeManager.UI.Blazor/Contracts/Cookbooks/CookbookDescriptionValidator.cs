using FluentValidation;

namespace RecipeManager.UI.Blazor.Contracts.Cookbooks;

public sealed class CookbookDescriptionValidator : AbstractValidator<string>
{
    public CookbookDescriptionValidator()
    {
        RuleFor(description => description)
            .MaximumLength(1000)
                .WithMessage("Cookbook description must not exceed 1000 characters.");
    }
}

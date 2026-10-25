using FluentValidation;
using RecipeManager.Api.Features.Validation;

namespace RecipeManager.Api.Features.Cookbooks.Command.CreateCookbook;

public sealed class CreateCookbookValidationPolicy : BaseValidator<CreateCookbookRequest>
{
    public CreateCookbookValidationPolicy()
    {
        RuleFor(request => request.Title).ValidateCookbookTitle();
        RuleFor(request => request.Description).ValidateCookbookDescription();
        RuleFor(request => request.Categories)
            .ValidateCookbookCategoryNames(category => category.Name, category => category.Subcategories);
    }
}

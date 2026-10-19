using FluentValidation;
using FluentValidation.Results;

namespace RecipeManager.Api.Features.Validation;

internal static class CookbookValidationExtensions
{
    public static IRuleBuilderOptions<T, string> ValidateCookbookTitle<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                          .WithMessage("Cookbook title must not be empty.")
                          .MaximumLength(100)
                          .WithMessage("Cookbook title must not exceed 100 characters.");
    }

    public static IRuleBuilderOptions<T, string> ValidateCookbookDescription<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.MaximumLength(1000)
                          .WithMessage("Cookbook description must not exceed 1000 characters.");
    }

    public static IRuleBuilderOptionsConditions<T, string> ValidateCookbookCategoryName<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Custom((name, context) =>
        {
            if (string.IsNullOrEmpty(name))
            {
                context.AddFailure("Category name must not be empty.");
            }

            if (name?.Length > 100)
            {
                context.AddFailure("Category name must not exceed 100 characters.");
            }
        });
    }

    public static void ValidateCookbookCategoryNames<T, TCategory>(
        this IRuleBuilder<T, IEnumerable<TCategory>> ruleBuilder,
        Func<TCategory, string> nameSelector,
        Func<TCategory, IEnumerable<TCategory>> subcategoriesSelector)
    {
        InlineValidator<string> categoryNameValidator = new();
        categoryNameValidator.RuleFor(name => name).ValidateCookbookCategoryName();

        _ = ruleBuilder.Custom((categories, context) =>
        {
            if (categories is null)
            {
                context.AddFailure(context.PropertyPath, "Categories must not be null.");
                return;
            }

            ValidateCategories(categories, context.PropertyPath);

            void ValidateCategories(IEnumerable<TCategory>? categoriesToValidate, string propertyPath)
            {
                if (categoriesToValidate is null)
                {
                    return;
                }

                int index = 0;
                foreach (TCategory category in categoriesToValidate)
                {
                    ValidationResult result = categoryNameValidator.Validate(nameSelector(category));
                    foreach (ValidationFailure failure in result.Errors)
                    {
                        context.AddFailure($"{propertyPath}[{index}].Name", failure.ErrorMessage);
                    }

                    IEnumerable<TCategory> subcategories = subcategoriesSelector(category);
                    if (subcategories is null)
                    {
                        context.AddFailure($"{propertyPath}[{index}].Subcategories", "Subcategories must not be null.");
                    }
                    else
                    {
                        ValidateCategories(subcategories, $"{propertyPath}[{index}].Subcategories");
                    }

                    index++;
                }
            }
        });
    }
}

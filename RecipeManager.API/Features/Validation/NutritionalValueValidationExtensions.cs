using FluentValidation;

namespace RecipeManager.Api.Features.Validation;

internal static class NutritionalValueValidationExtensions
{
    public static IRuleBuilderOptions<T, int> ValidateNutritionalCalories<T>(
        this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder.GreaterThanOrEqualTo(0)
                          .WithMessage("Calories value cannot be negative.");
    }

    public static IRuleBuilderOptions<T, double> ValidateNutritionalCarbohydrates<T>(
        this IRuleBuilder<T, double> ruleBuilder)
    {
        return ruleBuilder.GreaterThanOrEqualTo(0)
                          .WithMessage("Carbohydrates value cannot be negative.");
    }

    public static IRuleBuilderOptions<T, double> ValidateNutritionalFats<T>(
        this IRuleBuilder<T, double> ruleBuilder)
    {
        return ruleBuilder.GreaterThanOrEqualTo(0)
                          .WithMessage("Fats value cannot be negative.");
    }

    public static IRuleBuilderOptions<T, double> ValidateNutritionalProteins<T>(
        this IRuleBuilder<T, double> ruleBuilder)
    {
        return ruleBuilder.GreaterThanOrEqualTo(0)
                          .WithMessage("Proteins value cannot be negative.");
    }

    public static IRuleBuilderOptions<T, int> ValidateNutritionalAmount<T>(
        this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder.GreaterThanOrEqualTo(0)
                          .WithMessage("Amount for which nutitional values are provided cannot be negative.");
    }
}

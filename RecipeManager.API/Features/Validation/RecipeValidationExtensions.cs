using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Recipes;

namespace RecipeManager.Api.Features.Validation;

internal static class RecipeValidationExtensions
{
    public static IRuleBuilderOptions<T, TValue> MustReferenceExistingRecipes<T, TValue>(
        this IRuleBuilder<T, TValue> ruleBuilder,
        IAppDbContext dbContext,
        Func<TValue, IEnumerable<Guid>> idSelector,
        string message)
    {
        return ruleBuilder.MustReferenceExistingIds(
            idSelector,
            (ids, cancellationToken) =>
            {
                IEnumerable<RecipeId> recipeIds = ids.Select(id => new RecipeId(id));
                return dbContext.Recipes.CountAsync(recipe => recipeIds.Contains(recipe.Id), cancellationToken);
            },
            message);
    }

    public static IRuleBuilderOptions<T, double> ValidateRecipeAmount<T>(
        this IRuleBuilder<T, double> ruleBuilder)
    {
        return ruleBuilder.GreaterThan(0)
                          .WithMessage("Recipe amount must be greater than 0.");
    }

    public static IRuleBuilderOptions<T, byte> ValidateRecipeNumberOfServings<T>(
        this IRuleBuilder<T, byte> ruleBuilder)
    {
        return ruleBuilder.InclusiveBetween((byte)1, (byte)255)
                          .WithMessage("Number of servings must be between 1 and 255");
    }

    public static IRuleBuilderOptions<T, string> ValidateRecipeDescription<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.MaximumLength(500);
    }

    public static IRuleBuilderOptions<T, string?> ValidateRecipeVideoUrl<T>(
        this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.MaximumLength(2000);
    }

    public static IRuleBuilderOptions<T, string?> ValidateRecipeImageUrl<T>(
        this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.MaximumLength(2000);
    }

    public static IRuleBuilderOptions<T, string> ValidateRecipeStepDescription<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.MaximumLength(1000);
    }

    public static IRuleBuilderOptions<T, string> ValidateRecipeTitle<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                          .MaximumLength(100);
    }
}

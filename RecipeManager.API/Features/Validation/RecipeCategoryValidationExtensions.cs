using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Recipes;

namespace RecipeManager.Api.Features.Validation;

internal static class RecipeCategoryValidationExtensions
{
    public static IRuleBuilderOptions<T, TValue> MustReferenceExistingRecipeCategories<T, TValue>(
        this IRuleBuilder<T, TValue> ruleBuilder,
        IAppDbContext dbContext,
        Func<TValue, IEnumerable<Guid>> idSelector,
        string message)
    {
        return ruleBuilder.MustReferenceExistingIds(
            idSelector,
            (ids, cancellationToken) =>
            {
                IEnumerable<RecipeCategoryId> categoryIds = ids.Select(id => new RecipeCategoryId(id));
                return dbContext.RecipeCategories.CountAsync(category => categoryIds.Contains(category.Id), cancellationToken);
            },
            message);
    }

    public static IRuleBuilderOptions<T, string> MustHaveUniqueRecipeCategoryName<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        IAppDbContext dbContext,
        string message)
    {
        return ruleBuilder.MustAsync(async (name, cancellationToken) =>
            !await dbContext.RecipeCategories.AnyAsync(category => category.Name == name, cancellationToken))
            .WithMessage(message);
    }

    public static IRuleBuilderOptions<T, string> ValidateRecipeCategoryName<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                          .WithMessage("Category name cannot be empty")
                          .MaximumLength(100)
                          .WithMessage("Category name cannot exceed 100 characters");
    }
}

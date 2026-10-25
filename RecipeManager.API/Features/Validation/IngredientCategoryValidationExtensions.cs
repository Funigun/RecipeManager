using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Ingredients;

namespace RecipeManager.Api.Features.Validation;

internal static class IngredientCategoryValidationExtensions
{
    public static IRuleBuilderOptions<T, Guid?> MustReferenceExistingIngredientCategory<T>(
        this IRuleBuilder<T, Guid?> ruleBuilder,
        IAppDbContext dbContext,
        string message)
    {
        return ruleBuilder.MustAsync(async (id, cancellationToken) =>
            id is null || await dbContext.IngredientCategories.AnyAsync(category => category.Id == new IngredientCategoryId(id.Value), cancellationToken))
            .WithMessage(message);
    }

    public static IRuleBuilderOptions<T, TValue> MustReferenceExistingIngredientCategories<T, TValue>(
        this IRuleBuilder<T, TValue> ruleBuilder,
        IAppDbContext dbContext,
        Func<TValue, IEnumerable<Guid>> idSelector,
        string message)
    {
        return ruleBuilder.MustReferenceExistingIds(
            idSelector,
            (ids, cancellationToken) =>
            {
                IEnumerable<IngredientCategoryId> categoryIds = ids.Select(id => new IngredientCategoryId(id));
                return dbContext.IngredientCategories.CountAsync(category => categoryIds.Contains(category.Id), cancellationToken);
            },
            message);
    }

    public static IRuleBuilderOptions<T, string> MustHaveUniqueIngredientCategoryName<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        IAppDbContext dbContext,
        string message)
    {
        return ruleBuilder.MustAsync(async (name, cancellationToken) =>
            !await dbContext.IngredientCategories.AnyAsync(category => category.Name == name, cancellationToken))
            .WithMessage(message);
    }

    public static IRuleBuilderOptions<T, string> ValidateIngredientCategoryName<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                          .WithMessage("Category name cannot be empty")
                          .MaximumLength(100)
                          .WithMessage("Category name cannot exceed 100 characters");
    }
}

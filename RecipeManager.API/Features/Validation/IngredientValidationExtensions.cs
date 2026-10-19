using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Database;
using RecipeManager.Api.Domain.Ingredients;

namespace RecipeManager.Api.Features.Validation;

internal static class IngredientValidationExtensions
{
    public static IRuleBuilderOptions<T, Guid?> MustReferenceExistingIngredient<T>(
        this IRuleBuilder<T, Guid?> ruleBuilder,
        IAppDbContext dbContext,
        string message)
    {
        return ruleBuilder.MustAsync(async (id, cancellationToken) =>
            id is null || await dbContext.Ingredients.AnyAsync(ingredient => ingredient.Id == new IngredientId(id.Value), cancellationToken))
            .WithMessage(message);
    }

    public static IRuleBuilderOptions<T, TValue> MustReferenceExistingIngredients<T, TValue>(
        this IRuleBuilder<T, TValue> ruleBuilder,
        IAppDbContext dbContext,
        Func<TValue, IEnumerable<Guid>> idSelector,
        string message)
    {
        return ruleBuilder.MustReferenceExistingIds(
            idSelector,
            (ids, cancellationToken) =>
            {
                IEnumerable<IngredientId> ingredientIds = ids.Select(id => new IngredientId(id));
                return dbContext.Ingredients.CountAsync(ingredient => ingredientIds.Contains(ingredient.Id), cancellationToken);
            },
            message);
    }

    public static IRuleBuilderOptions<T, string> ValidateIngredientName<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                          .WithMessage("Ingredient name cannot be empty.")
                          .MaximumLength(100)
                          .WithMessage("Ingredient name cannot exceed 100 characters.");
    }
}

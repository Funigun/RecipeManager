using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Units;

namespace RecipeManager.Api.Features.Validation;

internal static class UnitValidationExtensions
{
    public static IRuleBuilderOptions<T, Guid> MustReferenceExistingUnit<T>(this IRuleBuilder<T, Guid> ruleBuilder, IAppDbContext dbContext, string message)
    {
        return ruleBuilder.MustAsync((id, cancellationToken) =>
            dbContext.Units.AnyAsync(unit => unit.Id == new UnitId(id), cancellationToken))
            .WithMessage(message);
    }

    public static IRuleBuilderOptions<T, Guid?> MustReferenceExistingUnit<T>(
        this IRuleBuilder<T, Guid?> ruleBuilder,
        IAppDbContext dbContext,
        string message)
    {
        return ruleBuilder.MustAsync(async (id, cancellationToken) =>
            id is null || await dbContext.Units.AnyAsync(unit => unit.Id == new UnitId(id.Value), cancellationToken))
            .WithMessage(message);
    }

    public static IRuleBuilderOptions<T, IEnumerable<TItem>> MustReferenceExistingUnits<T, TItem>(
        this IRuleBuilder<T, IEnumerable<TItem>> ruleBuilder,
        IAppDbContext dbContext,
        Func<TItem, Guid> idSelector,
        string message)
    {
        return ruleBuilder.MustAsync(async (items, cancellationToken) =>
        {
            if (items is null)
            {
                return false;
            }

            List<UnitId> ids = items.Select(idSelector)
                                    .Select(id => new UnitId(id))
                                    .Distinct()
                                    .ToList();

            return ids.Count == 0 || await dbContext.Units.CountAsync(unit => ids.Contains(unit.Id), cancellationToken) == ids.Count;
        }).WithMessage(message);
    }

    public static IRuleBuilderOptions<T, string> MustHaveUniqueUnitName<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        IAppDbContext dbContext,
        string message)
    {
        return ruleBuilder.MustAsync(async (name, cancellationToken) =>
            !await dbContext.Units.AnyAsync(unit => unit.Name == name, cancellationToken))
            .WithMessage(message);
    }

    public static IRuleBuilderOptions<T, string> MustHaveUniqueUnitName<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        IAppDbContext dbContext,
        Func<UnitId> excludedIdSelector,
        string message)
    {
        return ruleBuilder.MustAsync(async (name, cancellationToken) =>
        {
            UnitId excludedId = excludedIdSelector();
            return !await dbContext.Units.AnyAsync(unit => unit.Name == name && unit.Id != excludedId, cancellationToken);
        }).WithMessage(message);
    }

    public static IRuleBuilderOptions<T, string?> MustHaveUniqueUnitShortName<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        IAppDbContext dbContext,
        string message)
    {
        return ruleBuilder.MustAsync(async (shortName, cancellationToken) =>
            shortName is null || !await dbContext.Units.AnyAsync(unit => unit.ShortName == shortName, cancellationToken))
            .WithMessage(message);
    }

    public static IRuleBuilderOptions<T, string?> MustHaveUniqueUnitShortName<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        IAppDbContext dbContext,
        Func<UnitId> excludedIdSelector,
        string message)
    {
        return ruleBuilder.MustAsync(async (shortName, cancellationToken) =>
        {
            if (shortName is null)
            {
                return true;
            }

            UnitId excludedId = excludedIdSelector();
            return !await dbContext.Units.AnyAsync(unit => unit.ShortName == shortName && unit.Id != excludedId, cancellationToken);
        }).WithMessage(message);
    }

    public static IRuleBuilderOptions<T, string> ValidateUnitName<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                          .WithMessage("Unit name cannot be empty")
                          .MaximumLength(50)
                          .WithMessage("Unit name cannot exceed 50 characters");
    }

    public static IRuleBuilderOptions<T, string?> ValidateUnitShortName<T>(
        this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                          .WithMessage("Short name cannot be empty")
                          .MaximumLength(25)
                          .WithMessage("Short name cannot exceed 25 characters")
                          .When(shortName => shortName is not null);
    }

    public static IRuleBuilderOptions<T, string> ValidateUnitPluralName<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                          .WithMessage("Plural name is required.")
                          .MaximumLength(55)
                          .WithMessage("Plural name must be at most 100 characters.");
    }

    public static IRuleBuilderOptions<T, string?> ValidateUnitPluralShortName<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.MaximumLength(30)
                          .WithMessage("Plural short name must be at most 20 characters.");
    }

    public static IRuleBuilderOptions<T, bool> ValidateUnitIsBaseUnit<T>(
        this IRuleBuilder<T, bool> ruleBuilder)
    {
        return ruleBuilder.NotNull()
                          .WithMessage("IsBaseUnit is required.");
    }
}

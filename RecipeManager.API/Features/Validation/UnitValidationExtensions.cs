using FluentValidation;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Database;
using RecipeManager.Api.Domain.Units;

namespace RecipeManager.Api.Features.Validation;

internal static class UnitValidationExtensions
{
    public static IRuleBuilderOptions<T, Guid> MustReferenceExistingUnit<T>(
        this IRuleBuilder<T, Guid> ruleBuilder,
        IUnitOfWork unitOfWork,
        string message)
    {
        return ruleBuilder.MustAsync((id, cancellationToken) =>
            unitOfWork.Units.AnyByIdAsync(new UnitId(id), cancellationToken))
            .WithMessage(message);
    }

    public static IRuleBuilderOptions<T, Guid?> MustReferenceExistingUnit<T>(
        this IRuleBuilder<T, Guid?> ruleBuilder,
        IUnitOfWork unitOfWork,
        string message)
    {
        return ruleBuilder.MustAsync(async (id, cancellationToken) =>
            id is null || await unitOfWork.Units.AnyByIdAsync(new UnitId(id.Value), cancellationToken))
            .WithMessage(message);
    }

    public static IRuleBuilderOptions<T, IEnumerable<TItem>> MustReferenceExistingUnits<T, TItem>(
        this IRuleBuilder<T, IEnumerable<TItem>> ruleBuilder,
        IUnitOfWork unitOfWork,
        Func<TItem, Guid> idSelector,
        string message)
    {
        return ruleBuilder.MustAsync(async (items, cancellationToken) =>
        {
            if (items is null)
            {
                return false;
            }

            IEnumerable<UnitId> ids = items.Select(idSelector)
                                           .Select(id => new UnitId(id))
                                           .Distinct();

            return await unitOfWork.Units.AreIdsValidAsync(ids, cancellationToken);
        }).WithMessage(message);
    }

    public static IRuleBuilderOptions<T, string> MustHaveUniqueUnitName<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        IUnitOfWork unitOfWork,
        string message)
    {
        return ruleBuilder.MustAsync(async (name, cancellationToken) =>
            !await unitOfWork.Units.AnyByNameAsync(name, cancellationToken: cancellationToken))
            .WithMessage(message);
    }

    public static IRuleBuilderOptions<T, string> MustHaveUniqueUnitName<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        IUnitOfWork unitOfWork,
        Func<UnitId> excludedIdSelector,
        string message)
    {
        return ruleBuilder.MustAsync(async (name, cancellationToken) =>
            !await unitOfWork.Units.AnyByNameAsync(name, excludedIdSelector(), cancellationToken))
            .WithMessage(message);
    }

    public static IRuleBuilderOptions<T, string> MustHaveUniqueUnitShortName<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        IUnitOfWork unitOfWork,
        string message)
    {
        return ruleBuilder.MustAsync(async (shortName, cancellationToken) =>
            !await unitOfWork.Units.AnyByShortNameAsync(shortName, cancellationToken: cancellationToken))
            .WithMessage(message);
    }

    public static IRuleBuilderOptions<T, string> MustHaveUniqueUnitShortName<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        IUnitOfWork unitOfWork,
        Func<UnitId> excludedIdSelector,
        string message)
    {
        return ruleBuilder.MustAsync(async (shortName, cancellationToken) =>
            !await unitOfWork.Units.AnyByShortNameAsync(shortName, excludedIdSelector(), cancellationToken))
            .WithMessage(message);
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

    public static IRuleBuilderOptions<T, string?> ValidateUnitPluralShortName<T>(
        this IRuleBuilder<T, string?> ruleBuilder)
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

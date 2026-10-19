using FluentValidation;

namespace RecipeManager.Api.Features.Validation;

internal static class CollectionValidationExtensions
{
    public static IRuleBuilderOptions<T, IEnumerable<TItem>> MustHaveUniqueIds<T, TItem>(
        this IRuleBuilder<T, IEnumerable<TItem>> ruleBuilder,
        Func<TItem, Guid> idSelector,
        string message)
    {
        return ruleBuilder.Must(items =>
        {
            if (items is null)
            {
                return false;
            }

            List<Guid> ids = items.Select(idSelector).ToList();
            return ids.Distinct().Count() == ids.Count;
        }).WithMessage(message);
    }

    public static IRuleBuilderOptions<T, TValue> MustReferenceExistingIds<T, TValue>(
        this IRuleBuilder<T, TValue> ruleBuilder,
        Func<TValue, IEnumerable<Guid>> idSelector,
        Func<IReadOnlyCollection<Guid>, CancellationToken, Task<int>> existingCount,
        string message)
    {
        return ruleBuilder.MustAsync(async (value, cancellationToken) =>
        {
            IEnumerable<Guid>? ids = idSelector(value);
            if (ids is null)
            {
                return false;
            }

            List<Guid> idList = ids.ToList();
            if (idList.Count == 0)
            {
                return true;
            }

            return await existingCount(idList, cancellationToken) == idList.Count;
        }).WithMessage(message);
    }
}

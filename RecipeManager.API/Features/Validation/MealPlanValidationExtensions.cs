using FluentValidation;

namespace RecipeManager.Api.Features.Validation;

internal static class MealPlanValidationExtensions
{
    public static IRuleBuilderOptions<T, IEnumerable<TItem>> MustHaveUniqueDates<T, TItem>(
        this IRuleBuilder<T, IEnumerable<TItem>> ruleBuilder,
        Func<TItem, DateTime> dateSelector,
        string message)
    {
        return ruleBuilder.Must(items =>
        {
            if (items is null)
            {
                return false;
            }

            List<DateTime> dates = items.Select(dateSelector).ToList();
            return dates.Distinct().Count() == dates.Count;
        }).WithMessage(message);
    }
}

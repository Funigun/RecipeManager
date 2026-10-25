using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.MealPlan;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.MealPlanner.Query.GetMealPlan;

public sealed record GetMealPlanRequest(DateTimeOffset Day);

public sealed record GetMealPlanQuery(GetMealPlanRequest Request);

internal sealed record GetMealPlanRecipeProjection(Guid Id, string Title, string? ImageUrl);

public sealed record GetMealPlanRecipeDto(Guid Id, string Title, string? ImageUrl)
{
    internal static GetMealPlanRecipeDto FromProjection(GetMealPlanRecipeProjection recipe) =>
        new(recipe.Id, recipe.Title, recipe.ImageUrl);
}

public sealed record GetMealPlanDayDto(Guid Id, DateTimeOffset Date, ICollection<GetMealPlanRecipeDto> Recipes);

public sealed record GetMealPlanWeekDto(ICollection<GetMealPlanDayDto> Days);

public sealed record GetMealPlanResponse(ICollection<GetMealPlanWeekDto> Weeks);

public sealed class GetMealPlanQueryHandler(IAppDbContext dbContext, ICurrentUser currentUser)
{
    public async Task<GetMealPlanResponse> Handle(GetMealPlanQuery query, CancellationToken cancellationToken)
    {
        Tuple<DateTimeOffset, DateTimeOffset> dateRange = CalculateMealPlanRange(query.Request.Day);

        Dictionary<DateTimeOffset, MealPlan> mealPlans = await dbContext.MealPlans.AsNoTracking()
            .Where(plan => plan.Date.Date >= dateRange.Item1 &&
                           plan.Date.Date <= dateRange.Item2 &&
                           plan.CreatedBy == currentUser.Id)
            .ToDictionaryAsync(plan => plan.Date, cancellationToken);

        IEnumerable<RecipeId> recipeIds = mealPlans.Values.SelectMany(plan => plan.Recipes).Distinct();
        List<GetMealPlanRecipeProjection> recipeProjections = await dbContext.Recipes.AsNoTracking()
            .Where(recipe => recipeIds.Contains(recipe.Id))
            .Select(recipe => new GetMealPlanRecipeProjection(recipe.Id.Value, recipe.Title, recipe.ImageURL))
            .ToListAsync(cancellationToken);
        IEnumerable<GetMealPlanRecipeDto> recipes = recipeProjections
            .Select(GetMealPlanRecipeDto.FromProjection);

        return new GetMealPlanResponse(MapToMonthlyPlanDto(dateRange, mealPlans, recipes));
    }

    private static Tuple<DateTimeOffset, DateTimeOffset> CalculateMealPlanRange(DateTimeOffset day)
    {
        DateTimeOffset startDate = day.AddDays(-(day.Day - 1));
        DateTimeOffset endDate = startDate.AddMonths(1).AddDays(-1);

        while (startDate.DayOfWeek != DayOfWeek.Monday)
        {
            startDate = startDate.AddDays(-1);
        }

        while (endDate.DayOfWeek != DayOfWeek.Sunday)
        {
            endDate = endDate.AddDays(1);
        }

        return new Tuple<DateTimeOffset, DateTimeOffset>(startDate.Date, endDate.Date);
    }

    private static ICollection<GetMealPlanWeekDto> MapToMonthlyPlanDto(
        Tuple<DateTimeOffset, DateTimeOffset> dateRange,
        Dictionary<DateTimeOffset, MealPlan> mealPlans,
        IEnumerable<GetMealPlanRecipeDto> recipes)
    {
        ICollection<GetMealPlanWeekDto> weeks = [];
        ICollection<GetMealPlanDayDto> week = [];

        for (DateTimeOffset date = dateRange.Item1; date <= dateRange.Item2; date = date.AddDays(1))
        {
            week = date.DayOfWeek == DayOfWeek.Monday ? [] : week;
            mealPlans.TryGetValue(date, out MealPlan? dayPlan);

            ICollection<GetMealPlanRecipeDto> recipesForDay = dayPlan is null
                ? []
                : recipes.Where(recipe => dayPlan.Recipes.Any(id => id.Value == recipe.Id)).ToList();
            week.Add(new GetMealPlanDayDto(dayPlan?.Id ?? Guid.Empty, date, recipesForDay));

            if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                weeks.Add(new GetMealPlanWeekDto(week.ToList()));
            }
        }

        return weeks;
    }
}

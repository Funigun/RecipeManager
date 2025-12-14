using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.MealPlan;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Api.Shared.Hateoas.Builder;
using RecipeManager.Api.Shared.Hateoas.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RecipeManager.Api.Features.MealPlanner;

public static class GetMealPlan
{
    public sealed record Request(DateTimeOffset Day);

    public sealed record RecipeDto(Guid Id, string Title, string? ImageUrl);

    public sealed record MealPlanDayDto(Guid Id, DateTimeOffset Date, ICollection<RecipeDto> Recipes);

    public sealed record MealPlanWeekDto(ICollection<MealPlanDayDto> Days);

    public sealed record Response(ICollection<MealPlanWeekDto> Weeks);

    [GroupEndpoint("MealPlans")]
    public sealed class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardGet<Response>("/monthly", Handler)
                     .WithName("GetMealPlanForMonth")
                     .WithDescription("Returns meal plan for month including days before/after firts/last day of month to present whole week");
        }
    }

    internal static async Task<Results<Ok<Response>, BadRequest>> Handler([AsParameters] Request request, [FromServices] ICurrentUser currentUser, [FromServices] IAppDbContext dbContext, [FromServices] IHateoasBuilderFactory hateoasBuilderFactory, CancellationToken cancellationToken)
    {
        Tuple<DateTimeOffset, DateTimeOffset> dateRange = CalculateMealPlanRange(request.Day);

        Dictionary<DateTimeOffset, MealPlan> mealPlans = await dbContext.MealPlans.AsNoTracking()
                                                                                  .Where(plan => plan.Date.Date >= dateRange.Item1 &&
                                                                                                        plan.Date.Date <= dateRange.Item2 &&
                                                                                                        plan.CreatedBy == currentUser.Id)
                                                                                  .ToDictionaryAsync(plan => plan.Date, cancellationToken);

        IEnumerable<RecipeId> recipeIds = mealPlans.Values.SelectMany(plan => plan.Recipes).Distinct();

        IEnumerable<RecipeDto> recipes = await dbContext.Recipes.AsNoTracking()
                                                                .Where(recipe => recipeIds.Contains(recipe.Id))
                                                                .Select(recipe => new RecipeDto(recipe.Id, recipe.Title, recipe.ImageURL))
                                                                .ToListAsync(cancellationToken);

        ICollection<MealPlanWeekDto> monthlyPlanDto = MapToMonthlyPlanDto(dateRange, mealPlans, recipes);

        return TypedResults.Ok(new Response(monthlyPlanDto));
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

        return new(startDate.Date, endDate.Date);
    }

    private static ICollection<MealPlanWeekDto> MapToMonthlyPlanDto(Tuple<DateTimeOffset, DateTimeOffset> dateRange, Dictionary<DateTimeOffset, MealPlan> mealPlans, IEnumerable<RecipeDto> recipes)
    {
        ICollection<MealPlanWeekDto> weeks = [];
        ICollection<MealPlanDayDto> week = [];

        for (DateTimeOffset date = dateRange.Item1; date <= dateRange.Item2; date = date.AddDays(1))
        {
            week = date.DayOfWeek == DayOfWeek.Monday ? [] : week;

            mealPlans.TryGetValue(date, out MealPlan? dayPlan);

            ICollection<RecipeDto> recipesForDay = dayPlan == null ? [] : recipes.Where(recipe => dayPlan.Recipes.Any(r => r.Value == recipe.Id)).ToList();
            week.Add(new(dayPlan?.Id ?? Guid.Empty, date, recipesForDay));

            if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                weeks.Add(new(week.ToList()));
            }
        }

        return weeks;
    }
}

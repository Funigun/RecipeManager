using MudBlazor;
using RecipeManager.UI.Blazor.Brokers.RecipeManagersApi;
using RecipeManager.UI.Blazor.Features.MealPlan.Models;

namespace RecipeManager.UI.Blazor.Features.MealPlan.Services;

public sealed class MealPlanService(IRecipeApi recipeApi, ISnackbar snackbar) : IMealPlanService
{
    private const string MealPlansApiUrl = "api/mealplans";

    public async Task<MealPlannerMonth> GetMonthlyMealPlan(DateTimeOffset day)
    {
        HttpResponseMessage response = await recipeApi.GetAll(new Uri($"{MealPlansApiUrl}/monthly?day={day.ToString("yyyy-MM-ddThh:mm:ss")}", UriKind.Relative));

        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<MealPlannerMonth>())!;
        }

        return new MealPlannerMonth();
    }
}

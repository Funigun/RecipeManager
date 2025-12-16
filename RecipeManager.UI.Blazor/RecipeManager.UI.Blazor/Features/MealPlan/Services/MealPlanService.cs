using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using RecipeManager.UI.Blazor.Brokers.RecipeManagersApi;
using RecipeManager.UI.Blazor.Components.Common;
using RecipeManager.UI.Blazor.Features.MealPlan.Models;

namespace RecipeManager.UI.Blazor.Features.MealPlan.Services;

public sealed class MealPlanService(IRecipeApi recipeApi, ISnackbar snackbar, NavigationManager navigationManager) : IMealPlanService
{
    private const string MealPlansApiUrl = "api/mealplans";

    public ApiResponseBody ResponseBody { get; private set; } = new();

    public async Task<MealPlannerMonth> GetMonthlyMealPlan(DateTimeOffset day)
    {
        HttpResponseMessage response = await recipeApi.GetAll(new Uri($"{MealPlansApiUrl}/monthly?day={day.ToString("yyyy-MM-ddThh:mm:ss")}", UriKind.Relative));

        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<MealPlannerMonth>())!;
        }

        return new MealPlannerMonth();
    }

    public async Task UpdateMealPlan(IEnumerable<MealPlanForUpdateModel> mealPlans)
    {
        HttpResponseMessage response = await recipeApi.Update(new Uri($"{MealPlansApiUrl}", UriKind.Relative), mealPlans);

        if (response.IsSuccessStatusCode)
        {
            navigationManager.Refresh(true);
            snackbar.Add("Meal plan updated successfully.", Severity.Success);
        }
        else
        {
            ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;
            throw new Exception(JsonSerializer.Serialize(ResponseBody));
        }
    }
}

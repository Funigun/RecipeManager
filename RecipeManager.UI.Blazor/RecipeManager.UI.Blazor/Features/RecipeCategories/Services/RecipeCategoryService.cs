using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using RecipeManager.UI.Blazor.Brokers.HateoasModel;
using RecipeManager.UI.Blazor.Brokers.RecipeManagersApi;
using RecipeManager.UI.Blazor.Components.Common;
using RecipeManager.UI.Blazor.Components.Extensions;
using RecipeManager.UI.Blazor.Features.RecipeCategories.Models;

namespace RecipeManager.UI.Blazor.Features.RecipeCategories.Services;

public sealed class RecipeCategoryService(IRecipeApi recipeApi, ISnackbar snackbar, NavigationManager navigationManager) : IRecipeCategoryService
{
    private const string CategoriesApiUrl = "api/recipecategories";
    private const string CategoriesDropdownApiUrl = $"{CategoriesApiUrl}/dropdown";
    private const string CategoriesForFilteringApiUrl = $"{CategoriesApiUrl}/filtering";

    public ApiResponseBody ResponseBody { get; private set; } = new();

    public async Task<HateoasResponse<RecipeCategoryPageModel>> GetCategories(int pageNumber, int pageSie, int? categoryType)
    {
        string url = $"{CategoriesApiUrl}?page={pageNumber}&pageSize={pageSie}";

        if (categoryType is not null)
        {
            url += $"&categoryType={categoryType}";
        }

        HttpResponseMessage response = await recipeApi.GetAll(new Uri(url, UriKind.Relative));

        if (response.IsSuccessStatusCode)
        {
            string resp = await response.Content.ReadAsStringAsync();
            return (await response.Content.ReadFromJsonAsync<HateoasResponse<RecipeCategoryPageModel>>())!;
        }

        ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;

        return new HateoasResponse<RecipeCategoryPageModel>();
    }

    public async Task<IEnumerable<RecipeCategoryForDropdownModel>> GetCategoriesForDropdown(string? categoryName = null)
    {
        string url = string.IsNullOrEmpty(categoryName) ? CategoriesDropdownApiUrl : $"{CategoriesDropdownApiUrl}?categoryName={categoryName}";
        HttpResponseMessage response = await recipeApi.GetAll(new Uri(url, UriKind.Relative));

        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<IEnumerable<RecipeCategoryForDropdownModel>>())!;
        }

        ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;

        return [];
    }

    public async Task<Dictionary<RecipeCategoryType, IEnumerable<RecipeCategoryForDropdownModel>>> GetCategoriesForFiltering()
    {
        HttpResponseMessage response = await recipeApi.GetAll(new Uri(CategoriesForFilteringApiUrl, UriKind.Relative));

        if (response.IsSuccessStatusCode)
        {
            string jsonValue = await response.Content.ReadAsStringAsync();
            using JsonDocument? doc = JsonDocument.Parse(jsonValue);
            JsonElement categories = doc.RootElement.GetProperty("categories");
            JsonSerializerOptions opt = new() { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<Dictionary<RecipeCategoryType, IEnumerable<RecipeCategoryForDropdownModel>>>(categories.GetRawText(), opt)!;
        }

        ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;

        return RecipeCategoryTypeExtensions.GetCategoryTypesForRecipeFiltering();
    }

    public async Task CreateCategory(RecipeCategoryForCreateModel category)
    {
        HttpResponseMessage response = await recipeApi.Create(new Uri(CategoriesApiUrl, UriKind.Relative), category);

        if (response.IsSuccessStatusCode)
        {
            navigationManager.Refresh(true);
            snackbar.ShowSuccess("Category created sucessfully");
        }
        else
        {
            ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;
            snackbar.ShowError(string.Join('\n', ResponseBody.GetAllErrors()));
        }
    }

    public async Task DeleteCategory(string relativeUri)
    {
        HttpResponseMessage response = await recipeApi.DeleteItem(new Uri(relativeUri));

        if (response.IsSuccessStatusCode)
        {
            navigationManager.Refresh(true);
            snackbar.ShowSuccess("Category deleted sucessfully");
        }
        else
        {
            ResponseBody = (await response.Content.ReadFromJsonAsync<ApiResponseBody>())!;
        }
    }
}

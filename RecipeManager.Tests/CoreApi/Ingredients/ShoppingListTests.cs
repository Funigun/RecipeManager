using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Features.Ingredients;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.Ingredients;

public sealed class ShoppingListTests(WebApiFactory webApiFactory) : BaseIntegrationTest(webApiFactory)
{
    [Fact]
    public async Task GenerateShoppingList_ShouldReturnAuthoredRecipeAmounts()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));
        RecipeId recipeId = (await DbContext.Recipes.AsNoTracking().FirstAsync(recipe => recipe.Title == "Simple tomato mix", TestContext.Current.CancellationToken)).Id;

        GenerateShoppingList.Request requestDto = new([new GenerateShoppingList.RecipeDto(recipeId.Value)]);

        using StringContent content = new(JsonSerializer.Serialize(requestDto), System.Text.Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PutAsync("/api/ingredients/generate-shopping-list", content, TestContext.Current.CancellationToken);
        string responseBody = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        GenerateShoppingList.Response? shoppingListResponse = System.Text.Json.JsonSerializer.Deserialize<GenerateShoppingList.Response>(responseBody, JsonOptions);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(shoppingListResponse);

        // Simple tomato mix should have 2 ingredients: Canned tomatoes and Salt
        List<GenerateShoppingList.IngredientDto>? allIngredients = shoppingListResponse.Ingredients.SelectMany(x => x.Value).ToList();
        Assert.Equal(2, allIngredients.Count);

        GenerateShoppingList.IngredientDto? cannedTomatoes = allIngredients.FirstOrDefault(i => i.Name.Contains("Canned tomatoes"));
        GenerateShoppingList.IngredientDto? salt = allIngredients.FirstOrDefault(i => i.Name.Contains("Salt"));

        Assert.NotNull(cannedTomatoes);
        Assert.NotNull(salt);

        Assert.Equal(1, cannedTomatoes.Quantity);
        Assert.Contains("can", cannedTomatoes.Unit, StringComparison.OrdinalIgnoreCase);

        Assert.Equal(1, salt.Quantity);
        Assert.Contains("kg", salt.Unit, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GenerateShoppingList_ShouldAggregateRepeatedRecipes_UsingAuthoredAmounts()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));
        Recipe recipe = await DbContext.Recipes.AsNoTracking().FirstAsync(r => r.Title == "Simple tomato mix", TestContext.Current.CancellationToken);

        GenerateShoppingList.Request requestDto = new([
            new GenerateShoppingList.RecipeDto(recipe.Id.Value),
            new GenerateShoppingList.RecipeDto(recipe.Id.Value)
        ]);
        using StringContent content = new(JsonSerializer.Serialize(requestDto), System.Text.Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PutAsync("/api/ingredients/generate-shopping-list", content, TestContext.Current.CancellationToken);
        string responseBody = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        GenerateShoppingList.Response? shoppingListResponse = JsonSerializer.Deserialize<GenerateShoppingList.Response>(responseBody, JsonOptions);

        // Assert
        Assert.NotNull(shoppingListResponse);
        List<GenerateShoppingList.IngredientDto> allIngredients = shoppingListResponse.Ingredients.SelectMany(x => x.Value).ToList();
        GenerateShoppingList.IngredientDto? cannedTomatoes = allIngredients.FirstOrDefault(i => i.Name.Contains("Canned tomatoes"));
        GenerateShoppingList.IngredientDto? salt = allIngredients.FirstOrDefault(i => i.Name.Contains("Salt"));

        Assert.NotNull(cannedTomatoes);
        Assert.NotNull(salt);
        Assert.Equal(2, cannedTomatoes.Quantity);
        Assert.Contains("can", cannedTomatoes.Unit, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(1, salt.Quantity);
        Assert.Contains("kg", salt.Unit, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GenerateShoppingList_ShouldAggregateIngredientAmounts_WhenMultipleRecipesContainSameIngredient()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));
        Recipe simpleTomatoMix = await DbContext.Recipes.AsNoTracking().FirstAsync(r => r.Title == "Simple tomato mix", TestContext.Current.CancellationToken);
        Recipe complexTomatoMix = await DbContext.Recipes.AsNoTracking().FirstAsync(r => r.Title == "Complex tomato mix", TestContext.Current.CancellationToken);

        GenerateShoppingList.Request requestDto = new([
            new GenerateShoppingList.RecipeDto(simpleTomatoMix.Id.Value),
            new GenerateShoppingList.RecipeDto(complexTomatoMix.Id.Value)
        ]);
        using StringContent content = new(JsonSerializer.Serialize(requestDto), System.Text.Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PutAsync("/api/ingredients/generate-shopping-list", content, TestContext.Current.CancellationToken);
        string responseBody = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        GenerateShoppingList.Response? shoppingListResponse = JsonSerializer.Deserialize<GenerateShoppingList.Response>(responseBody, JsonOptions);
        Assert.NotNull(shoppingListResponse);
        List<GenerateShoppingList.IngredientDto> allIngredients = shoppingListResponse.Ingredients.SelectMany(x => x.Value).ToList();

        // Assert: 3 cans of tomatoes (1 from Simple, 1.5 from Complex rounded up to 2)
        GenerateShoppingList.IngredientDto? cannedTomatoes = allIngredients.FirstOrDefault(i => i.Name.Contains("Canned tomatoes"));
        Assert.NotNull(cannedTomatoes);
        Assert.Equal(3, cannedTomatoes.Quantity);
        Assert.Contains("can", cannedTomatoes.Unit, StringComparison.OrdinalIgnoreCase);

        // Optionally, check salt and other ingredients as needed
    }

    [Fact]
    public async Task GenerateShoppingList_ShouldHandleDifferentUnits_ForSameIngredientAcrossRecipes()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));

        // Load recipes by title seeded in DatabaseSeeder: "Pizza dough" and "Burger buns"
        Recipe pizzaDough = await DbContext.Recipes.AsNoTracking().FirstAsync(r => r.Title == "Pizza dough", TestContext.Current.CancellationToken);
        Recipe burgerBuns = await DbContext.Recipes.AsNoTracking().FirstAsync(r => r.Title == "Burger buns", TestContext.Current.CancellationToken);

        // Use their original servings to pick the intended ingredient quantities
        GenerateShoppingList.Request requestDto = new([
            new GenerateShoppingList.RecipeDto(pizzaDough.Id.Value),
            new GenerateShoppingList.RecipeDto(burgerBuns.Id.Value)
        ]);

        using StringContent content = new(JsonSerializer.Serialize(requestDto), System.Text.Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PutAsync("/api/ingredients/generate-shopping-list", content, TestContext.Current.CancellationToken);
        string responseBody = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        GenerateShoppingList.Response? shoppingListResponse = JsonSerializer.Deserialize<GenerateShoppingList.Response>(responseBody, JsonOptions);

        // Assert
        Assert.NotNull(shoppingListResponse);

        List<GenerateShoppingList.IngredientDto> allIngredients = shoppingListResponse.Ingredients.SelectMany(x => x.Value).ToList();

        // Find flour ingredient which appears in both recipes but with different units (grams vs cups)
        GenerateShoppingList.IngredientDto? flour = allIngredients.FirstOrDefault(i => i.Name.Contains("Flour", StringComparison.OrdinalIgnoreCase));
        Assert.NotNull(flour);

        // Expected behavior: conversions applied (4 cups ->480g) +300g ->780g -> purchases in kilograms (package size1kg) -> rounded up to1
        Assert.Equal(1, flour.Quantity);
        Assert.Contains("kg", flour.Unit, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GenerateShoppingList_ShouldExpandNestedRecipes_UsingAuthoredRecipeAmounts()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));
        Recipe mexicanSoup = await DbContext.Recipes.AsNoTracking().FirstAsync(r => r.Title == "Mexican soup", TestContext.Current.CancellationToken);

        GenerateShoppingList.Request requestDto = new([
            new GenerateShoppingList.RecipeDto(mexicanSoup.Id.Value)
        ]);
        using StringContent content = new(JsonSerializer.Serialize(requestDto), System.Text.Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PutAsync("/api/ingredients/generate-shopping-list", content, TestContext.Current.CancellationToken);
        string responseBody = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        GenerateShoppingList.Response? shoppingListResponse = JsonSerializer.Deserialize<GenerateShoppingList.Response>(responseBody, JsonOptions);
        Assert.NotNull(shoppingListResponse);
        List<GenerateShoppingList.IngredientDto> allIngredients = shoppingListResponse.Ingredients.SelectMany(x => x.Value).ToList();

        // Assert chicken (kg)
        GenerateShoppingList.IngredientDto? chicken = allIngredients.FirstOrDefault(i => i.Name.Contains("chicken", StringComparison.OrdinalIgnoreCase));
        Assert.NotNull(chicken);
        Assert.Equal(1, chicken.Quantity, 2);
        Assert.Contains("kg", chicken.Unit, StringComparison.OrdinalIgnoreCase);

        // Assert carrot (g)
        GenerateShoppingList.IngredientDto? carrot = allIngredients.FirstOrDefault(i => i.Name.Contains("carrot", StringComparison.OrdinalIgnoreCase));
        Assert.NotNull(carrot);
        Assert.Equal(1, carrot.Quantity, 2);
        Assert.Contains("kg", carrot.Unit, StringComparison.OrdinalIgnoreCase);

        // Assert onion (g)
        GenerateShoppingList.IngredientDto? onion = allIngredients.FirstOrDefault(i => i.Name.Contains("onion", StringComparison.OrdinalIgnoreCase));
        Assert.NotNull(onion);
        Assert.Equal(1, onion.Quantity, 2);
        Assert.Contains("kg", onion.Unit, StringComparison.OrdinalIgnoreCase);

        // Assert water (L)
        GenerateShoppingList.IngredientDto? water = allIngredients.FirstOrDefault(i => i.Name.Contains("water", StringComparison.OrdinalIgnoreCase));
        Assert.NotNull(water);
        Assert.Equal(2, water.Quantity, 2);
        Assert.Contains("l", water.Unit, StringComparison.OrdinalIgnoreCase);
    }
}

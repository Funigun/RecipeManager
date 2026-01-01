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
    public async Task GenerateShoppingList_ShouldReturnExactRecipeAmounts_WhenNumberOfServingMatchesRecipeServings()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));
        RecipeId recipeId = (await DbContext.Recipes.AsNoTracking().FirstAsync(receipe => receipe.Title == "Simple tomato mix", TestContext.Current.CancellationToken)).Id;
        int servings = (await DbContext.Recipes.AsNoTracking().FirstAsync(receipe => receipe.Title == "Simple tomato mix", TestContext.Current.CancellationToken)).NumberOfServings;

        GenerateShoppingList.Request requestDto = new([new GenerateShoppingList.RecipeDto(recipeId.Value, servings)]);

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
    public async Task GenerateShoppingList_ShouldReturnScaledRecipeAmounts_WhenNumberOfServingDiffersFromRecipeServings()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));
        Recipe recipe = await DbContext.Recipes.AsNoTracking().FirstAsync(r => r.Title == "Simple tomato mix", TestContext.Current.CancellationToken);

        // Test scaling down to 1 serving
        GenerateShoppingList.Request requestDtoDown = new GenerateShoppingList.Request([
            new GenerateShoppingList.RecipeDto(recipe.Id.Value, 1)
        ]);
        using StringContent contentDown = new StringContent(JsonSerializer.Serialize(requestDtoDown), System.Text.Encoding.UTF8, "application/json");
        HttpResponseMessage responseDown = await HttpClient.PutAsync("/api/ingredients/generate-shopping-list", contentDown, TestContext.Current.CancellationToken);
        responseDown.EnsureSuccessStatusCode();
        string responseBodyDown = await responseDown.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        GenerateShoppingList.Response? shoppingListDown = JsonSerializer.Deserialize<GenerateShoppingList.Response>(responseBodyDown, JsonOptions);
        Assert.NotNull(shoppingListDown);
        List<GenerateShoppingList.IngredientDto> allIngredientsDown = shoppingListDown.Ingredients.SelectMany(x => x.Value).ToList();
        Assert.Equal(2, allIngredientsDown.Count);
        GenerateShoppingList.IngredientDto? cannedTomatoesDown = allIngredientsDown.FirstOrDefault(i => i.Name.Contains("Canned tomatoes"));
        GenerateShoppingList.IngredientDto? saltDown = allIngredientsDown.FirstOrDefault(i => i.Name.Contains("Salt"));
        Assert.NotNull(cannedTomatoesDown);
        Assert.NotNull(saltDown);
        Assert.Equal(1, cannedTomatoesDown.Quantity); // can't buy half a can
        Assert.Contains("can", cannedTomatoesDown.Unit, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(1, saltDown.Quantity); // always 1 package
        Assert.Contains("kg", saltDown.Unit, StringComparison.OrdinalIgnoreCase);

        // Test scaling up to 3 servings
        GenerateShoppingList.Request requestDtoUp = new GenerateShoppingList.Request([
            new GenerateShoppingList.RecipeDto(recipe.Id.Value, 3)
        ]);
        using StringContent contentUp = new StringContent(JsonSerializer.Serialize(requestDtoUp), System.Text.Encoding.UTF8, "application/json");
        HttpResponseMessage responseUp = await HttpClient.PutAsync("/api/ingredients/generate-shopping-list", contentUp, TestContext.Current.CancellationToken);
        responseUp.EnsureSuccessStatusCode();
        string responseBodyUp = await responseUp.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        GenerateShoppingList.Response? shoppingListUp = JsonSerializer.Deserialize<GenerateShoppingList.Response>(responseBodyUp, JsonOptions);
        Assert.NotNull(shoppingListUp);
        List<GenerateShoppingList.IngredientDto> allIngredientsUp = shoppingListUp.Ingredients.SelectMany(x => x.Value).ToList();
        Assert.Equal(2, allIngredientsUp.Count);
        GenerateShoppingList.IngredientDto? cannedTomatoesUp = allIngredientsUp.FirstOrDefault(i => i.Name.Contains("Canned tomatoes"));
        GenerateShoppingList.IngredientDto? saltUp = allIngredientsUp.FirstOrDefault(i => i.Name.Contains("Salt"));
        Assert.NotNull(cannedTomatoesUp);
        Assert.NotNull(saltUp);
        Assert.Equal(2, cannedTomatoesUp.Quantity); // 1.5 cans rounds up to 2
        Assert.Contains("can", cannedTomatoesUp.Unit, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(1, saltUp.Quantity); // still 1 package (7.5g < 1kg)
        Assert.Contains("kg", saltUp.Unit, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GenerateShoppingList_ShouldAggregateIngredientAmounts_WhenMultipleRecipesContainSameIngredient()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));
        Recipe simpleTomatoMix = await DbContext.Recipes.AsNoTracking().FirstAsync(r => r.Title == "Simple tomato mix", TestContext.Current.CancellationToken);
        Recipe complexTomatoMix = await DbContext.Recipes.AsNoTracking().FirstAsync(r => r.Title == "Complex tomato mix", TestContext.Current.CancellationToken);

        GenerateShoppingList.Request requestDto = new GenerateShoppingList.Request([
            new GenerateShoppingList.RecipeDto(simpleTomatoMix.Id.Value, simpleTomatoMix.NumberOfServings),
            new GenerateShoppingList.RecipeDto(complexTomatoMix.Id.Value, complexTomatoMix.NumberOfServings)
        ]);
        using StringContent content = new StringContent(JsonSerializer.Serialize(requestDto), System.Text.Encoding.UTF8, "application/json");

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
        GenerateShoppingList.Request requestDto = new GenerateShoppingList.Request([
            new GenerateShoppingList.RecipeDto(pizzaDough.Id.Value, pizzaDough.NumberOfServings),
            new GenerateShoppingList.RecipeDto(burgerBuns.Id.Value, burgerBuns.NumberOfServings)
        ]);

        using StringContent content = new StringContent(JsonSerializer.Serialize(requestDto), System.Text.Encoding.UTF8, "application/json");

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

    [Theory]
    [InlineData(2, 1, 1, 1, 1)]
    [InlineData(4, 1, 1, 1, 2)]
    [InlineData(6, 2, 1, 1, 3)]
    public async Task GenerateShoppingList_ShouldHandleNestedRecipes_ForIngredientThatHasItsOwnRecipe(int servings, double expectedChickenKg, double expectedCarrotG, double expectedOnionG, double expectedWaterL)
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));
        Recipe mexicanSoup = await DbContext.Recipes.AsNoTracking().FirstAsync(r => r.Title == "Mexican soup", TestContext.Current.CancellationToken);

        GenerateShoppingList.Request requestDto = new GenerateShoppingList.Request([
            new GenerateShoppingList.RecipeDto(mexicanSoup.Id.Value, servings)
        ]);
        using StringContent content = new StringContent(JsonSerializer.Serialize(requestDto), System.Text.Encoding.UTF8, "application/json");

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
        Assert.Equal(expectedChickenKg, chicken.Quantity, 2); // 2 decimals precision
        Assert.Contains("kg", chicken.Unit, StringComparison.OrdinalIgnoreCase);

        // Assert carrot (g)
        GenerateShoppingList.IngredientDto? carrot = allIngredients.FirstOrDefault(i => i.Name.Contains("carrot", StringComparison.OrdinalIgnoreCase));
        Assert.NotNull(carrot);
        Assert.Equal(expectedCarrotG, carrot.Quantity, 2);
        Assert.Contains("kg", carrot.Unit, StringComparison.OrdinalIgnoreCase);

        // Assert onion (g)
        GenerateShoppingList.IngredientDto? onion = allIngredients.FirstOrDefault(i => i.Name.Contains("onion", StringComparison.OrdinalIgnoreCase));
        Assert.NotNull(onion);
        Assert.Equal(expectedOnionG, onion.Quantity, 2);
        Assert.Contains("kg", onion.Unit, StringComparison.OrdinalIgnoreCase);

        // Assert water (L)
        GenerateShoppingList.IngredientDto? water = allIngredients.FirstOrDefault(i => i.Name.Contains("water", StringComparison.OrdinalIgnoreCase));
        Assert.NotNull(water);
        Assert.Equal(expectedWaterL, water.Quantity, 2);
        Assert.Contains("l", water.Unit, StringComparison.OrdinalIgnoreCase);
    }
}

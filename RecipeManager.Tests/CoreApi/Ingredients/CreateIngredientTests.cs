using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Features.Ingredients;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.Ingredients;

[Trait("Core.Api", "Ingredients")]
public sealed class CreateIngredientTests : BaseIntegrationTest
{
    public CreateIngredientTests(WebApiFactory webApiFactory) : base(webApiFactory)
    {
    }

    [Fact]
    public async Task CreateIngredient_ShouldReturn_CategoryId_ForValidInput()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));
        IngredientCategoryId categoryId = await DbContext.IngredientCategories.AsNoTracking()
                                                                              .Select(c => c.Id)
                                                                              .FirstAsync(TestContext.Current.CancellationToken);

        UnitId existingUnitId = await DbContext.Units.AsNoTracking().Where(u => u.Name == "Existing Unit")
                                                     .Select(u => u.Id)
                                                     .FirstAsync(TestContext.Current.CancellationToken);

        CreateIngredient.NutritionalValueDto nutritionalValue = new(100, 5, 10, 20, 1, existingUnitId.Value);
        CreateIngredient.Request request = new("New fake ingredient", nutritionalValue, categoryId, [], []);
        using StringContent content = new(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsync("/api/ingredients", content, TestContext.Current.CancellationToken);
        string responseBody = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        CreateIngredient.Response? ingredientId = JsonSerializer.Deserialize<CreateIngredient.Response>(responseBody, JsonOptions);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(ingredientId);
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Contains($"/api/ingredients/{ingredientId.Id}", response.Headers.Location!.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateIngredient_ShouldReturn_BadRequest_ForMissingName()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));
        IngredientCategoryId categoryId = await DbContext.IngredientCategories.AsNoTracking()
                                                                              .Select(c => c.Id)
                                                                              .FirstAsync(TestContext.Current.CancellationToken);

        UnitId existingUnitId = await DbContext.Units.AsNoTracking().Where(u => u.Name == "Existing Unit")
                                                     .Select(u => u.Id)
                                                     .FirstAsync(TestContext.Current.CancellationToken);

        CreateIngredient.NutritionalValueDto nutritionalValue = new(100, 5, 10, 20, 1, existingUnitId.Value);
        CreateIngredient.Request? request = new(string.Empty, nutritionalValue, categoryId, [], []);
        using StringContent content = new(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsync("/api/ingredients", content, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateIngredient_ShouldReturn_BadRequest_ForMissingShoppingCategoryId()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));

        UnitId existingUnitId = await DbContext.Units.AsNoTracking().Where(u => u.Name == "Existing Unit")
                                                     .Select(u => u.Id)
                                                     .FirstAsync(TestContext.Current.CancellationToken);

        CreateIngredient.NutritionalValueDto nutritionalValue = new(100, 5, 10, 20, 1, existingUnitId.Value);
        CreateIngredient.Request? request = new(string.Empty, nutritionalValue, Guid.Empty, [], []);
        using StringContent content = new(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsync("/api/ingredients", content, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateIngredient_ShouldReturn_BadRequest_ForWrongCategoryId()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));

        IngredientCategoryId categoryId = await DbContext.IngredientCategories.AsNoTracking()
                                                                              .Select(c => c.Id)
                                                                              .FirstAsync(TestContext.Current.CancellationToken);

        UnitId existingUnitId = await DbContext.Units.AsNoTracking().Where(u => u.Name == "Existing Unit")
                                                     .Select(u => u.Id)
                                                     .FirstAsync(TestContext.Current.CancellationToken);

        CreateIngredient.NutritionalValueDto nutritionalValue = new(100, 5, 10, 20, 1, existingUnitId.Value);
        CreateIngredient.Request? request = new(string.Empty, nutritionalValue, categoryId, [Guid.Empty], []);
        using StringContent content = new(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsync("/api/ingredients", content, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateIngredient_ShouldReturn_BadRequest_ForWrongRecipeId()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));

        IngredientCategoryId categoryId = await DbContext.IngredientCategories.AsNoTracking()
                                                                              .Select(c => c.Id)
                                                                              .FirstAsync(TestContext.Current.CancellationToken);

        UnitId existingUnitId = await DbContext.Units.AsNoTracking().Where(u => u.Name == "Existing Unit")
                                                     .Select(u => u.Id)
                                                     .FirstAsync(TestContext.Current.CancellationToken);

        CreateIngredient.NutritionalValueDto nutritionalValue = new(100, 5, 10, 20, 1, existingUnitId.Value);
        CreateIngredient.Request? request = new(string.Empty, nutritionalValue, categoryId, [], [Guid.Empty]);
        using StringContent content = new(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsync("/api/ingredients", content, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateIngredient_ShouldReturn_BadRequest_ForDuplicateShoppingListAndCategoryId()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));

        IngredientCategoryId categoryId = await DbContext.IngredientCategories.AsNoTracking()
                                                                              .Select(c => c.Id)
                                                                              .FirstAsync(TestContext.Current.CancellationToken);

        UnitId existingUnitId = await DbContext.Units.AsNoTracking().Where(u => u.Name == "Existing Unit")
                                                     .Select(u => u.Id)
                                                     .FirstAsync(TestContext.Current.CancellationToken);

        CreateIngredient.NutritionalValueDto nutritionalValue = new(100, 5, 10, 20, 1, existingUnitId.Value);
        CreateIngredient.Request? request = new(string.Empty, nutritionalValue, categoryId, [categoryId], []);
        using StringContent content = new(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsync("/api/ingredients", content, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(-10, 5, 10, 20, 1, "Negative calories")]
    [InlineData(100, -5, 10, 20, 1, "Negative proteins")]
    [InlineData(100, 5, -10, 20, 1, "Negative fats")]
    [InlineData(100, 5, 10, -20, 1, "Negative carbohydrates")]
    [InlineData(100, 5, 10, 20, -1, "Negative amount")]
    public async Task CreateIngredient_ShouldReturn_BadRequest_ForInvalidNutritionalValues(int calories, double proteins, double fats, double carbohydrates, int amount, string justification)
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));
        IngredientCategoryId categoryId = await DbContext.IngredientCategories.AsNoTracking()
                                                                              .Select(c => c.Id)
                                                                              .FirstAsync(TestContext.Current.CancellationToken);

        UnitId existingUnitId = await DbContext.Units.AsNoTracking().Where(u => u.Name == "Existing Unit")
                                                     .Select(u => u.Id)
                                                     .FirstAsync(TestContext.Current.CancellationToken);

        CreateIngredient.NutritionalValueDto nutritionalValue = new(calories, proteins, fats, carbohydrates, amount, existingUnitId.Value);
        CreateIngredient.Request? request = new("Valid Name", nutritionalValue, categoryId, [], []);
        using StringContent content = new(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsync("/api/ingredients", content, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
}

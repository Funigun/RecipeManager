using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RecipeManager.Api.Features.IngredientCategories;
using RecipeManager.Api.Features.RecipeCategories;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.IngredientCategories;

[Trait("Core.Api", "IngredientCategories")]
public sealed class CreateIngredientCategoryTests : BaseIntegrationTest
{
    public CreateIngredientCategoryTests(WebApiFactory webApiFactory) : base(webApiFactory)
    {
    }

    [Fact]
    public async Task CreateRecipeCategory_ShouldReturn_NotAuthorized_ForNonAdminUser()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedUser()));

        CreateIngredientCategory.Request createCategoryRequest = new("Ingredient category");
        StringContent content = new(JsonSerializer.Serialize(createCategoryRequest), Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsync("/api/ingredientCategories", content, CancellationToken.None);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [InlineData("", "Ingredient category name must be between 1 and 100 characters long.")]
    [InlineData("Existing Category", "Ingredient category must be unique")]
    public async Task CreateRecipeCategory_ShouldReturn_BadRequest_ForInvalidInput(string categoryName, string justification)
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));

        CreateIngredientCategory.Request createCategoryRequest = new(categoryName);
        StringContent content = new(JsonSerializer.Serialize(createCategoryRequest), Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsync("/api/ingredientCategories", content, CancellationToken.None);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateRecipeCategory_ShouldReturn_CategoryId_ForValidInput()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));

        CreateIngredientCategory.Request createCategoryRequest = new("New Recipe Category");
        StringContent content = new(JsonSerializer.Serialize(createCategoryRequest), Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsync("/api/ingredientCategories", content, CancellationToken.None);
        string responseBody = await response.Content.ReadAsStringAsync();
        CreateRecipeCategory.Response? categoryId = JsonSerializer.Deserialize<CreateRecipeCategory.Response>(responseBody, JsonOptions);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(categoryId);
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Contains($"/api/ingredientCategories/{categoryId.Id}", response.Headers.Location!.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}

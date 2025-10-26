using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RecipeManager.Api.Features.RecipeCategories;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.RecipeCategories;

[Trait("Core.Api", "RecipeCategories")]
public sealed class CreateCategoryTests : BaseIntegrationTest
{
    public CreateCategoryTests(WebApiFactory webApiFactory) : base(webApiFactory)
    {
    }

    [Fact]
    public async Task CreateRecipeCategory_ShouldReturn_NotAuthorized_ForNonAdminUser()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedUser()));

        CreateRecipeCategory.Request createUnitRequest = new("Recipe category", 1);
        StringContent content = new(JsonSerializer.Serialize(createUnitRequest), Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsync("/api/recipeCategories", content, CancellationToken.None);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [InlineData("", 1, "Recipe category name must be between 1 and 100 characters long.")]
    [InlineData("Existing Category", 1, "Recipe category must be unique")]
    [InlineData("Some random category", -1, "Invalid recipe category type")]
    public async Task CreateRecipeCategory_ShouldReturn_BadRequest_ForInvalidInput(string categoryName, int categoryType, string justification)
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));

        CreateRecipeCategory.Request createCategoryRequest = new(categoryName, categoryType);
        StringContent content = new(JsonSerializer.Serialize(createCategoryRequest), Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsync("/api/recipeCategories", content, CancellationToken.None);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateRecipeCategory_ShouldReturn_CategoryId_ForValidInput()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));

        CreateRecipeCategory.Request createCategoryRequest = new("New Recipe Category", 1);
        StringContent content = new(JsonSerializer.Serialize(createCategoryRequest), Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsync("/api/recipeCategories", content, CancellationToken.None);
        string responseBody = await response.Content.ReadAsStringAsync();
        CreateRecipeCategory.Response? categoryId = JsonSerializer.Deserialize<CreateRecipeCategory.Response>(responseBody, JsonOptions);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(categoryId);
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Contains($"/api/recipeCategories/{categoryId.Id}", response.Headers.Location!.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}

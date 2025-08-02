using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.RecipeCategories;

public sealed class DeleteCategoryTests : BaseIntegrationTest
{
    public DeleteCategoryTests(WebApiFactory webApiFactory) : base(webApiFactory)
    {
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturn_NotAuthorized_ForNonAdminUser()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedUser()));
        Guid guid = Guid.CreateVersion7();

        // Act
        HttpResponseMessage response = await HttpClient.DeleteAsync($"/api/recipeCategories/{guid}", CancellationToken.None);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturn_NotFound_ForNonExistingCategory()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));
        Guid guid = Guid.CreateVersion7();

        // Act
        HttpResponseMessage response = await HttpClient.DeleteAsync($"/api/recipeCategories/{guid}", CancellationToken.None);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturn_NoContent_ForExistingCategory()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));
        Guid categoryId = (await DbContext.RecipeCategories.AsNoTracking().FirstAsync(category => category.Name == "To Delete", CancellationToken.None)).Id.Value;

        // Act
        HttpResponseMessage deleteResponse = await HttpClient.DeleteAsync($"/api/recipeCategories/{categoryId}", CancellationToken.None);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }
}

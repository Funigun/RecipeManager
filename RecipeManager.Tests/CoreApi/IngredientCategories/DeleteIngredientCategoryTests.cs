using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.IngredientCategories;

[Trait("Core.Api", "IngredientCategories")]
public sealed class DeleteIngredientCategoryTests : BaseIntegrationTest
{
    public DeleteIngredientCategoryTests(WebApiFactory webApiFactory) : base(webApiFactory)
    {
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturn_NotAuthorized_ForNonAdminUser()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedUser()));
        Guid guid = Guid.CreateVersion7();

        // Act
        HttpResponseMessage response = await HttpClient.DeleteAsync($"/api/ingredientCategories/{guid}", TestContext.Current.CancellationToken);

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
        HttpResponseMessage response = await HttpClient.DeleteAsync($"/api/ingredientCategories/{guid}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturn_NoContent_ForExistingCategory()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));
        Guid categoryId = (await DbContext.IngredientCategories.AsNoTracking().FirstAsync(category => category.Name == "To Delete", TestContext.Current.CancellationToken)).Id.Value;

        // Act
        HttpResponseMessage deleteResponse = await HttpClient.DeleteAsync($"/api/ingredientCategories/{categoryId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }
}

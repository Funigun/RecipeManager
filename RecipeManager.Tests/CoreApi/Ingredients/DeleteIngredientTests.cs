using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.Ingredients;

[Trait("Core.Api", "Ingredients")]
public sealed class DeleteIngredientTests : BaseIntegrationTest
{
    public DeleteIngredientTests(WebApiFactory webApiFactory) : base(webApiFactory)
    {
    }

    [Fact]
    public async Task DeleteIngredient_ShouldReturn_NoContent_ForExistingIngredient()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));
        Guid ingredientId = (await DbContext.Ingredients.AsNoTracking().FirstAsync(ingredient => ingredient.Name == "To Delete", CancellationToken.None)).Id.Value;

        // Act
        HttpResponseMessage deleteResponse = await HttpClient.DeleteAsync($"/api/ingredients/{ingredientId}", CancellationToken.None);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }
}

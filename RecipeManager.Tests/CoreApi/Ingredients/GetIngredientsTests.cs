using System.Net.Http.Headers;
using System.Text.Json;
using RecipeManager.Api.Features.Ingredients;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.Ingredients;

[Trait("Core.Api", "Ingredients")]
public sealed class GetIngredientsTests : BaseIntegrationTest
{
    public GetIngredientsTests(WebApiFactory webApiFactory) : base(webApiFactory)
    {
    }

    [Fact]
    public async Task GetIngredients_ShouldReturnProperModel_ForExistingIngredient()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedUser()));

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync($"/api/ingredients", CancellationToken.None);
        string responseBody = await response.Content.ReadAsStringAsync();
        GetIngredients.Response? ingredientResponse = JsonSerializer.Deserialize<GetIngredients.Response>(responseBody, JsonOptions);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(ingredientResponse);
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }
}

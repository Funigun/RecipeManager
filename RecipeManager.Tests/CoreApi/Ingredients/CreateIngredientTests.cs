using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
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

        CreateIngredient.Request request = new("New fake ingredient", [], []);
        StringContent content = new(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsync("/api/ingredients", content, CancellationToken.None);
        string responseBody = await response.Content.ReadAsStringAsync();
        CreateIngredient.Response? ingredientId = JsonSerializer.Deserialize<CreateIngredient.Response>(responseBody, JsonOptions);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(ingredientId);
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Contains($"/api/ingredients/{ingredientId.Id}", response.Headers.Location!.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}

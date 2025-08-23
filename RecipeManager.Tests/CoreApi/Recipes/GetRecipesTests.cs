using System.Net.Http.Headers;
using System.Text.Json;
using RecipeManager.Api.Features.Recipes;
using RecipeManager.Api.Shared.Hateoas.Models;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.Recipes;

[Trait("Core.Api", "Recipes")]
public sealed class GetRecipesTests : BaseIntegrationTest
{
    public GetRecipesTests(WebApiFactory webApiFactory) : base(webApiFactory)
    {
    }

    [Fact]
    public async Task GetRecipes_ShouldReturnAllRecipes_WhenNoFiltersApplied()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync("/api/recipes", CancellationToken.None);
        string responseBody = await response.Content.ReadAsStringAsync();
        HateoasResponse<GetRecipes.Response> recipesResponse = JsonSerializer.Deserialize<HateoasResponse<GetRecipes.Response>>(responseBody, JsonOptions);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(recipesResponse);
    }
}

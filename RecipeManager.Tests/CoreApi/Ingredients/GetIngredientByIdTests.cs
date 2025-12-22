using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Features.Ingredients;
using RecipeManager.Api.Shared.Hateoas.Models;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.Ingredients;

[Trait("Core.Api", "Ingredients")]
public sealed class GetIngredientByIdTests : BaseIntegrationTest
{
    public GetIngredientByIdTests(WebApiFactory webApiFactory) : base(webApiFactory)
    {
    }

    [Fact]
    public async Task GetIngredientById_ShouldReturnProperModel_ForExistingIngredient()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedUser()));
        Guid ingredientId = (await DbContext.Ingredients.AsNoTracking().FirstAsync(ingredient => ingredient.Name == "Existing Ingredient", TestContext.Current.CancellationToken)).Id.Value;

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync($"/api/ingredients/{ingredientId}", TestContext.Current.CancellationToken);
        string responseBody = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        HateoasResponse<GetIngredientById.Response>? ingredientResponse = JsonSerializer.Deserialize<HateoasResponse<GetIngredientById.Response>>(responseBody, JsonOptions);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(ingredientResponse);
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(ingredientId, ingredientResponse.Item.Id);
        Assert.NotNull(ingredientResponse.Item.IngredientPackage);
        Assert.True(ingredientResponse.Item.IngredientPackage.PackageSize > 0);
    }
}

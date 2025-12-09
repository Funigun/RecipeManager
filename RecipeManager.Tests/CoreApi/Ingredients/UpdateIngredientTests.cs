using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Features.Ingredients;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.Ingredients;

[Trait("Core.Api", "Ingredients")]
public sealed class UpdateIngredientTests : BaseIntegrationTest
{
    public UpdateIngredientTests(WebApiFactory webApiFactory) : base(webApiFactory)
    {
    }

    [Fact]
    public async Task UpdateIngredient_ShouldReturn_Ok_ForValidInput()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));
        Guid ingredientId = (await DbContext.Ingredients.AsNoTracking().FirstAsync(ingredient => ingredient.Name == "Existing Ingredient", TestContext.Current.CancellationToken)).Id.Value;
        Guid ingredientCategoryId = (await DbContext.IngredientCategories.AsNoTracking().FirstAsync(category => category.Name == "Existing Category", TestContext.Current.CancellationToken)).Id.Value;
        Guid unitId = (await DbContext.Units.AsNoTracking().FirstAsync(unit => unit.Name == "Existing Unit", TestContext.Current.CancellationToken)).Id.Value;

        UpdateIngredient.NutritionalValueDto nutritionalValue = new(100, 1, 1, 1, 1, unitId);
        UpdateIngredient.IngredientDto request = new("Update Name", nutritionalValue, ingredientCategoryId, [], []);
        using StringContent content = new(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PutAsync($"/api/ingredients/{ingredientId}", content, TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }
}

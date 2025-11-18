using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Features.Recipes;
using RecipeManager.Api.Shared.Hateoas.Models;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.Recipes;

[Trait("Core.Api", "Recipes")]
public sealed class GetRecipeByIdTests : BaseIntegrationTest
{
    public GetRecipeByIdTests(WebApiFactory webApiFactory) : base(webApiFactory)
    {
    }

    [Fact]
    public async Task GetRecipeById_ShouldReturnRecipe_ForValidId()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));
        RecipeId recipeId = (await DbContext.Recipes.AsNoTracking().FirstAsync(recipe => recipe.Title == "Existing recipe", TestContext.Current.CancellationToken)).Id.Value;

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync($"/api/recipes/{recipeId}", TestContext.Current.CancellationToken);
        string responseBody = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        HateoasResponse<GetRecipeById.Response>? recipeResponse = JsonSerializer.Deserialize<HateoasResponse<GetRecipeById.Response>>(responseBody, JsonOptions);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(recipeResponse);
    }
}

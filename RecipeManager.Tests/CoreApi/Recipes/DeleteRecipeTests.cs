using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.Recipes;

[Trait("Core.Api", "Recipes")]
public sealed class DeleteRecipeTests : BaseIntegrationTest
{
    public DeleteRecipeTests(WebApiFactory webApiFactory) : base(webApiFactory)
    {
    }

    [Fact]
    public async Task DeleteRecipe_ShouldReturnNoContent_ForValidId()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));
        RecipeId recipeId = (await DbContext.Recipes.AsNoTracking().FirstAsync(recipe => recipe.Title == "To Delete", TestContext.Current.CancellationToken)).Id.Value;

        // Act
        HttpResponseMessage response = await HttpClient.DeleteAsync($"/api/recipes/{recipeId}", TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
    }
}

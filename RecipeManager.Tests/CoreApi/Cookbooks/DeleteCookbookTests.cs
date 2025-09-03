using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Domain.Cookbooks;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.Cookbooks;

[Trait("Core.Api", "Cookbooks")]
public sealed class DeleteCookbookTests : BaseIntegrationTest
{
    public DeleteCookbookTests(WebApiFactory webApiFactory) : base(webApiFactory)
    {
    }

    [Fact]
    public async Task DeleteCookbook_ShouldReturnNoContent_ForExistingCookbook()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));
        CookbookId cookbookId = (await DbContext.Cookbooks.AsNoTracking().Where(cookbook => cookbook.Title == "To Delete").FirstAsync(CancellationToken.None)).Id;

        // Act
        HttpResponseMessage response = await HttpClient.DeleteAsync($"/api/cookbooks/{cookbookId.Value}", CancellationToken.None);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
    }
}

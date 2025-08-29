using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Domain.Cookbooks;
using RecipeManager.Api.Features.Cookbooks;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.Cookbooks;

[Trait("Core.Api", "Cookbooks")]
public sealed class UpdateCookbookTests : BaseIntegrationTest
{
    public UpdateCookbookTests(WebApiFactory webApiFactory) : base(webApiFactory)
    {
    }

    [Fact]
    public async Task UpdateCookbook_ShouldUpdateCookbook_ForValidData()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedUser()));
        Cookbook cookbook = await DbContext.Cookbooks.FirstAsync(cookbook => cookbook.Title == "To update", CancellationToken.None);

        UpdateCookbook.CookbookDto dto = new("Updated title", []);
        StringContent content = new(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PutAsync($"/api/cookbooks/{cookbook.Id}", content, CancellationToken.None);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }
}

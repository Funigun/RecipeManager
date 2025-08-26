using System.Net.Http.Json;
using RecipeManager.Api.Features.Cookbooks;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.Cookbooks;

[Trait("Core.Api", "Cookbooks")]
public sealed class CreateCookbookTests : BaseIntegrationTest
{
    public CreateCookbookTests(WebApiFactory webApiFactory) : base(webApiFactory)
    {
    }

    [Fact]
    public async Task CreateCookbook_ShouldReturnId_ForValidInput()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));
        CreateCookbook.Request createCookbookRequest = new("New Cookbook", []);
        StringContent content = new(System.Text.Json.JsonSerializer.Serialize(createCookbookRequest), System.Text.Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsync("/api/cookbooks", content, CancellationToken.None);

        // Assert
        response.EnsureSuccessStatusCode();
        CreateCookbook.Response? responseData = await response.Content.ReadFromJsonAsync<CreateCookbook.Response>(cancellationToken: CancellationToken.None);
        Assert.NotNull(responseData);
        Assert.NotEqual(Guid.Empty, responseData.Id);
    }
}

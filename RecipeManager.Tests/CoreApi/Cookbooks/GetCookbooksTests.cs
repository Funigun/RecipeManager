using RecipeManager.Api.Features.Cookbooks;
using RecipeManager.Api.Shared.Hateoas.Models;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.Cookbooks;

[Trait("Core.Api", "Cookbooks")]
public sealed class GetCookbooksTests : BaseIntegrationTest
{
    public GetCookbooksTests(WebApiFactory webApiFactory) : base(webApiFactory)
    {
    }

    [Fact]
    public async Task GetCookbooks_ShouldReturnCookbooksList()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedUser()));

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync("/api/cookbooks", TestContext.Current.CancellationToken);
        string responseContent = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        HateoasResponse<GetCookbooks.Response>? responseModel = System.Text.Json.JsonSerializer.Deserialize<HateoasResponse<GetCookbooks.Response>>(responseContent, JsonOptions);

        // Assert
        Assert.NotNull(responseModel);
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }
}

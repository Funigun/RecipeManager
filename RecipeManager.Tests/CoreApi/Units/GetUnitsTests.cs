using System.Net.Http.Headers;
using System.Text.Json;
using RecipeManager.Api.Shared.Hateoas.Models;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.Units;

[Trait("Core.Api", "Units")]
public sealed class GetUnitsTests : BaseIntegrationTest
{
    public GetUnitsTests(WebApiFactory apiFactory) : base(apiFactory)
    {
    }

    [Fact]
    public async Task GetUnits_ShouldReturn_NotAuthorized_WhenUserIsNotLoggedIn()
    {
        // Act
        HttpResponseMessage response = await HttpClient.GetAsync("/api/units", CancellationToken.None);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetUnits_ShouldReturn_Units_WithoutLinks_ForNonAdminUser()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedUser()));

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync("/api/units", CancellationToken.None);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        string content = await response.Content.ReadAsStringAsync(CancellationToken.None);
        Assert.NotNull(content);

        HateoasCollectionResponse<GetUnit.Response>? units = JsonSerializer.Deserialize<HateoasCollectionResponse<GetUnit.Response>>(content, JsonOptions);
        Assert.NotNull(units);
        Assert.NotEmpty(units.Items);
        Assert.All(units.Items, unit =>
        {
            Assert.Empty(unit.Links);
        });
        Assert.Empty(units.Links);
    }

    [Fact]
    public async Task GetUnits_ShouldReturn_Units_WithLinks_ForAdminUser()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync("/api/units", CancellationToken.None);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        string content = await response.Content.ReadAsStringAsync(CancellationToken.None);
        Assert.NotNull(content);

        HateoasCollectionResponse<GetUnit.Response>? units = JsonSerializer.Deserialize<HateoasCollectionResponse<GetUnit.Response>>(content, JsonOptions);
        Assert.NotNull(units);
        Assert.NotEmpty(units.Links);
        Assert.NotEmpty(units.Items);

        Assert.All(units.Items, unit =>
        {
            Assert.NotEmpty(unit.Links);
        });
    }
}

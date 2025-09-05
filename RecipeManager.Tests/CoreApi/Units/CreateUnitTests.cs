using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RecipeManager.Api.Features.Units;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.Units;

[Trait("Core.Api", "Units")]
public sealed class CreateUnitTests : BaseIntegrationTest
{
    public CreateUnitTests(WebApiFactory apiFactory) : base(apiFactory)
    {

    }

    [Fact]
    public async Task CreateUnit_ShouldReturn_NotAuthorized_ForNonAdminUser()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedUser()));

        CreateUnit.Request createUnitRequest = new("Valid Unit", "VA", 0, null);
        StringContent content = new(JsonSerializer.Serialize(createUnitRequest), Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsync("/api/units", content, CancellationToken.None);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [InlineData("", "TU", 1, "Missing Unit Name")]
    [InlineData("Test", "", 1, "Missing Short Name")]
    [InlineData("Another Unit", "AU", 100, "Invalid Unit Group")]
    [InlineData("Duplicated Name", null, 1, "Duplicate Unit Name")]
    [InlineData("Test", "Duplicated Short Name", 1, "Duplicate Unit Short Name")]
    public async Task CreateUnit_ShouldReturn_BadRequest_ForInvalidInput(string unitName, string? shortName, int group, string justification)
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));

        CreateUnit.Request createUnitRequest = new(unitName, shortName, group, null);
        StringContent content = new(JsonSerializer.Serialize(createUnitRequest), Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsync("/api/units", content, CancellationToken.None);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateUnit_ShouldReturn_UnitId_ForValidInput()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));

        CreateUnit.Request createUnitRequest = new("Valid Unit", "VA", 0, null);
        StringContent content = new(JsonSerializer.Serialize(createUnitRequest), Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsync("/api/units", content, CancellationToken.None);
        string responseContent = await response.Content.ReadAsStringAsync(CancellationToken.None);
        CreateUnit.Response? unitId = JsonSerializer.Deserialize<CreateUnit.Response?>(responseContent, JsonOptions);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(unitId);
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Contains($"/api/units/{unitId.Id}", response.Headers.Location!.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}

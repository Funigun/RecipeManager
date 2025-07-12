using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RecipeManager.Api.Features.Units;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.Units;

[Trait("Core.Api", "Units")]
public sealed class UpdateUnitTests : BaseIntegrationTest
{
    public UpdateUnitTests(WebApiFactory apiFactory) : base(apiFactory)
    {
    }

    [Fact]
    public async Task UpdateUnit_ShouldReturn_NotAuthorized_ForNonAdminUser()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedUser()));

        Guid unitId = Guid.CreateVersion7();
        UpdateUnit.Request updateUnitRequest = new("Updated Unit", "UU", 0);
        StringContent content = new(JsonSerializer.Serialize(updateUnitRequest), Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PutAsync($"/api/units/{unitId}", content, CancellationToken.None);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [InlineData("", "TU", 1, "Missing Unit Name")]
    [InlineData("Test", "", 1, "Missing Short Name")]
    [InlineData("Another Unit", "AU", 100, "Invalid Unit Group")]
    [InlineData("Duplicated Name", null, 1, "Duplicate Unit Name")]
    [InlineData("Test", "Duplicated Short Name", 1, "Duplicate Unit Short Name")]
    public async Task UpdateUnit_ShouldReturn_BadRequest_ForInvalidInput(string unitName, string? shortName, int group, string justification)
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));

        Guid unitId = DbContext.Units.Select(unit => unit.Id.Value).First();
        UpdateUnit.Request updateUnitRequest = new(unitName, shortName, group);
        StringContent content = new(JsonSerializer.Serialize(updateUnitRequest), Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PutAsync($"/api/units/{unitId}", content, CancellationToken.None);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateUnit_ShouldReturn_NoContent_ForValidInput()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));

        Guid unitId = DbContext.Units.Select(unit => unit.Id.Value).First();
        UpdateUnit.Request updateUnitRequest = new("Updated Unit", "UU", 0);
        StringContent content = new(JsonSerializer.Serialize(updateUnitRequest), Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PutAsync($"/api/units/{unitId}", content, CancellationToken.None);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
    }
}

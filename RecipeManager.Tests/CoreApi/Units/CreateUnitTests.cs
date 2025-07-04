using System.Net.Http.Headers;
using System.Net.Http.Json;
using RecipeManager.Integration.Factory.Tests.Common.Users;
using RecipeManager.Integration.Factory.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Factory.Tests.CoreApi.Units;

[Trait("Core.Api", "Units")]
public sealed class CreateUnitTests : BaseIntegrationTest
{
    public CreateUnitTests(DbContainerFactory dockerServicesFactory) : base(dockerServicesFactory)
    {
    }

    [Fact]
    public async Task CreateUnit_ShouldReturn_NotAuthorized_ForNonAdminUser()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedUser()));

        var createUnitRequest = new
        {
            Name = "Test Unit",
            ShortName = "TU",
            Group = 1
        };

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("/api/units", createUnitRequest, CancellationToken.None);

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

        var createUnitRequest = new
        {
            Name = unitName,
            ShortName = shortName,
            Group = group
        };

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("/api/units", createUnitRequest, CancellationToken.None);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
}

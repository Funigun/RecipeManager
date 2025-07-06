using System.Net.Http.Headers;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.Units;

[Trait("Core.Api", "Units")]
public sealed class DeleteUnitTests : BaseIntegrationTest
{
    public DeleteUnitTests(WebApiFactory apiFactory) : base(apiFactory)
    {
    }

    [Fact]
    public async Task DeleteUnit_ShouldReturn_NotAuthorized_ForNonAdminUser()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedUser()));
        Guid guid = Guid.CreateVersion7();

        // Act
        HttpResponseMessage response = await HttpClient.DeleteAsync($"/api/units/{guid}", CancellationToken.None);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUnit_ShouldReturn_NotFound_ForNonExistingUnit()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));
        Guid guid = Guid.CreateVersion7();

        // Act
        HttpResponseMessage response = await HttpClient.DeleteAsync($"/api/units/{guid}", CancellationToken.None);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUnit_ShouldReturn_NoContent_ForExistingUnit()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));

        Guid unitId = DbContext.Units.Select(unit => unit.Id).First();

        // Act
        HttpResponseMessage deleteResponse = await HttpClient.DeleteAsync($"/api/units/{unitId}", CancellationToken.None);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }
}

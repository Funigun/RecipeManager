using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Features.Units;
using RecipeManager.Api.Features.Units.Command.CreateUnit;
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

        CreateUnitRequest createUnitRequest = new(
            "Valid Unit", "VA", "Valid Units", "VAs", 0, true, null, 1
        );
        using StringContent content = new(JsonSerializer.Serialize(createUnitRequest), Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsync(UnitsGroup.Route, content, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [InlineData("", "TU", "", "", 1, true, "Missing Unit Name")]
    [InlineData("Test", "", "Tests", "", 1, true, "Missing Short Name")]
    [InlineData("Another Unit", "AU", "Another Units", "AUs", 100, true, "Invalid Unit Group")]
    [InlineData("Duplicated Name", null, "Duplicated Names", null, 1, true, "Duplicate Unit Name")]
    [InlineData("Test", "Duplicated Short Name", "Tests", "Duplicated Short Names", 1, true, "Duplicate Unit Short Name")]
    public async Task CreateUnit_ShouldReturn_BadRequest_ForInvalidInput(string unitName, string? shortName, string pluralName, string? pluralShortName, int group, bool isBaseUnit, string justification)
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));

        CreateUnitRequest createUnitRequest = new(unitName, shortName, pluralName, pluralShortName, group, isBaseUnit, null, 1);
        using StringContent content = new(JsonSerializer.Serialize(createUnitRequest), Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsync(UnitsGroup.Route, content, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(response.StatusCode == System.Net.HttpStatusCode.BadRequest, $"Expected BadRequest for justification: {justification}");
        ValidationProblemDetails? problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problem);
        Assert.NotEmpty(problem.Errors);
    }

    [Fact]
    public async Task CreateUnit_ShouldReturn_UnitId_ForValidInput()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));

        CreateUnitRequest createUnitRequest = new(
            "Valid Unit", "VA", "Valid Units", "VAs", 0, true, null, 1
        );
        using StringContent content = new(JsonSerializer.Serialize(createUnitRequest), Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsync(UnitsGroup.Route, content, TestContext.Current.CancellationToken);
        string responseContent = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        CreateUnitResponse? unitId = JsonSerializer.Deserialize<CreateUnitResponse?>(responseContent, JsonOptions);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(unitId);
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Contains(UnitsGroup.UnitById(unitId.Id), response.Headers.Location!.ToString(), StringComparison.OrdinalIgnoreCase);
        Assert.Equal("Valid Units", unitId.PluralName);
        Assert.Equal("VAs", unitId.PluralShortName);
        Assert.True(unitId.IsBaseUnit);
    }
}

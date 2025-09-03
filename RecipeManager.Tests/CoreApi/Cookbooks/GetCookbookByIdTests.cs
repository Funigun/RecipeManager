using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Domain.Cookbooks;
using RecipeManager.Api.Features.Cookbooks;
using RecipeManager.Api.Shared.Hateoas.Models;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.Cookbooks;

[Trait("Core.Api", "Cookbooks")]
public sealed class GetCookbookByIdTests : BaseIntegrationTest
{
    public GetCookbookByIdTests(WebApiFactory webApiFactory) : base(webApiFactory)
    {
    }

    [Fact]
    public async Task GetCookbookById_ShouldReturnCookbook_ForExistingCookbook()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));
        CookbookId cookbookId = (await DbContext.Cookbooks.AsNoTracking().Where(cookbook => cookbook.Title == "Test book").FirstAsync(CancellationToken.None)).Id;

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync($"/api/cookbooks/{cookbookId.Value}", CancellationToken.None);
        string responseContent = await response.Content.ReadAsStringAsync(CancellationToken.None);
        HateoasResponse<GetCookbookById.Response>? hateoasResponse = System.Text.Json.JsonSerializer.Deserialize<HateoasResponse<GetCookbookById.Response>>(responseContent, JsonOptions);

        // Assert
        Assert.NotNull(hateoasResponse);
    }
}

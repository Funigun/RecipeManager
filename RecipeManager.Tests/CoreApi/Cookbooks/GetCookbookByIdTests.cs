using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Domain.Cookbooks;
using RecipeManager.Api.Features.Cookbooks;
using RecipeManager.Api.Features.Cookbooks.Query.GetCookbookById;
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
        CookbookId cookbookId = (await DbContext.Cookbooks.AsNoTracking().Where(cookbook => cookbook.Title == "Test book").FirstAsync(TestContext.Current.CancellationToken)).Id;

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync(CookbooksGroup.CookbookById(cookbookId.Value), TestContext.Current.CancellationToken);
        string responseContent = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        HateoasResponse<GetCookbookByIdResponse>? hateoasResponse = System.Text.Json.JsonSerializer.Deserialize<HateoasResponse<GetCookbookByIdResponse>>(responseContent, JsonOptions);

        // Assert
        Assert.NotNull(hateoasResponse);
    }
}

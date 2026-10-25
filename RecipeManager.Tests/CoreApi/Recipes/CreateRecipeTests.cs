using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Recipes.Enums;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Features.Recipes;
using RecipeManager.Api.Features.Recipes.Command.CreateRecipe;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.Recipes;

[Trait("Core.Api", "Recipes")]
public sealed class CreateRecipeTests : BaseIntegrationTest
{
    public CreateRecipeTests(WebApiFactory webApiFactory) : base(webApiFactory)
    {
    }

    [Fact]
    public async Task CreateRecipe_ShouldReturnId_ForValidInput()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));
        CreateRecipeRequest request = await CreateFakeRecipe();
        using StringContent content = new(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PostAsync(RecipesGroup.Route, content, TestContext.Current.CancellationToken);
        string responseBody = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        CreateRecipeResponse? recipeId = JsonSerializer.Deserialize<CreateRecipeResponse>(responseBody, JsonOptions);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(recipeId);
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Contains(RecipesGroup.RecipeById(recipeId.Id), response.Headers.Location!.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    private async Task<CreateRecipeRequest> CreateFakeRecipe()
    {
        UnitId unitId = (await DbContext.Units.AsNoTracking().FirstAsync(TestContext.Current.CancellationToken)).Id;
        IngredientId ingredientId = (await DbContext.Ingredients.AsNoTracking().FirstAsync(TestContext.Current.CancellationToken)).Id;

        CreateRecipeAmountDto amount = new(10d, new(unitId.Value));
        CreateRecipeIngredientDto ingredientDto = new(new(ingredientId.Value, null), new(unitId.Value), 10d);
        CreateRecipeSectionDto ingredientsPreparation = new((int)RecipeSectionType.IngredientsPreparation, [new CreateRecipeStepDto(1, "Test", null)]);
        CreateRecipeSectionDto cooking = new((int)RecipeSectionType.Cooking, [new CreateRecipeStepDto(1, "Test 2", null)]);
        CreateRecipeNutritionalValueDto nutritionalValue = new(1, 1, 1, 1, 1, unitId.Value);

        return new CreateRecipeRequest
        (
            "New fake recipe",
            "This is a test recipe",
            null,
            null,
            amount,
            1,
            (int)RecipeDifficulty.Easy,
            nutritionalValue,
            [ingredientDto],
            [ingredientsPreparation, cooking],
            [],
            null
        );
    }
}

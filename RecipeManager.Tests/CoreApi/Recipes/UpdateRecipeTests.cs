using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Features.Recipes;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Integration.Tests.CoreApi.Recipes;

[Trait("Core.Api", "Recipes")]
public sealed class UpdateRecipeTests : BaseIntegrationTest
{
    public UpdateRecipeTests(WebApiFactory webApiFactory) : base(webApiFactory)
    {
    }

    [Fact]
    public async Task UpdateRecipe_ShouldReturnNoContent_ForValidUpdate()
    {
        // Arrange
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));

        Recipe recipe = await DbContext.Recipes.Include(r => r.Ingredients)
                                               .Include(r => r.Categories)
                                               .Include(r => r.Sections)
                                                .ThenInclude(s => s.Steps)
                                               .AsSplitQuery()
                                               .AsNoTracking()
                                               .FirstAsync(recipe => recipe.Title == "To Update", TestContext.Current.CancellationToken);

        UpdateRecipe.RecipeDto recipeDto = new
        (
            recipe.Title,
            "Updated description",
            null,
            null,
            new UpdateRecipe.RecipeAmountDto(500d, new(recipe.Amount.UnitId.Value)),
            5,
            (int)recipe.Difficulty,
            recipe.Ingredients.Select(ingredient => new UpdateRecipe.RecipeIngredientDto(new(ingredient.IngredientId, null), new(ingredient.UnitId), ingredient.Amount)).ToList(),
            recipe.Sections.Select(section => new UpdateRecipe.RecipeSectionDto((int)section.Type, section.Steps.Select(step => new UpdateRecipe.RecipeStepDto(step.Order, step.Description, null)).ToList())).ToList(),
            [],
            null
        );

        using StringContent content = new(JsonSerializer.Serialize(recipeDto), Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await HttpClient.PutAsync($"/api/recipes/{recipe.Id.Value}", content, TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }
}

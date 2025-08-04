using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.IngredientCategories;

public sealed class IngredientCategoryGroup : IGroupEndpoint
{
    public string GroupName { get; } = "IngredientCategories";

    public void Configure(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.WithDescription("Ingredient categories features")
                    .WithTags("Ingredient Categories")
                    .RequireAuthorization("RecipeManagerPolicy")
                    .ProducesProblem(StatusCodes.Status401Unauthorized);
    }
}

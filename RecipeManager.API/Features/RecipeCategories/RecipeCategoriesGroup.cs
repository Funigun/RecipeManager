using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.RecipeCategories;

public class RecipeCategoriesGroup : IGroupEndpoint
{
    public string GroupName { get; } = "RecipeCategories";

    public void Configure(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.WithDescription("Recipe categories features")
                    .WithTags("Recipe Categories")
                    .RequireAuthorization("RecipeManagerPolicy")
                    .ProducesProblem(StatusCodes.Status401Unauthorized);
    }
}

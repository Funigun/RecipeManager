using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Recipes;

public class RecipesGroup : IGroupEndpoint
{
    public string GroupName { get; } = "Recipes";

    public void Configure(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.WithDescription("Recipes features")
                    .WithTags("Recipes")
                    .RequireAuthorization("RecipeManagerPolicy")
                    .ProducesProblem(StatusCodes.Status401Unauthorized);
    }
}

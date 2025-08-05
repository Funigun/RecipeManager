using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Ingredients;

public sealed class IngredientsGroup : IGroupEndpoint
{
    public string GroupName { get; } = "Ingredients";

    public void Configure(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.WithDescription("Ingredients features")
                    .WithTags("Ingredients")
                    .RequireAuthorization("RecipeManagerPolicy")
                    .ProducesProblem(StatusCodes.Status401Unauthorized);
    }
}

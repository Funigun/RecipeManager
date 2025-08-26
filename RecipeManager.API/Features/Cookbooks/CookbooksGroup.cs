using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Cookbooks;

public sealed class CookbooksGroup : IGroupEndpoint
{
    public string GroupName { get; } = "Cookbooks";

    public void Configure(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization("RecipeManagerPolicy")
                    .WithDescription("Cookbooks features")
                    .WithTags("Cookbooks")
                    .ProducesProblem(StatusCodes.Status401Unauthorized);
    }
}

using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Api.Features.Units;

public class UnitsGroup : IGroupEndpoint
{
    public string GroupName { get; } = "Units";

    public void Configure(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.WithDescription("Measurement units features")
                    .WithTags("units")
                    .RequireAuthorization("RecipeManagerPolicy")
                    .ProducesProblem(StatusCodes.Status401Unauthorized);
    }
}

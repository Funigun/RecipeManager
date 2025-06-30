using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.API.Features.Units;

public class UnitsGroup : IGroupEndpoint
{
    public string GroupName { get; } = "Units";

    public void Configure(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.WithDescription("Measurement units features")
                    .WithTags("units");
    }
}

using Microsoft.AspNetCore.Routing;

namespace RecipeManager.Api.Shared.Endpoint;
public interface IGroupEndpoint
{
    string Name { get; }

    void Configure(RouteGroupBuilder groupBuilder);
}

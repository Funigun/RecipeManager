using Microsoft.AspNetCore.Routing;

namespace RecipeManager.Api.Shared.Endpoint;
public interface IGroupEndpoint
{
    string GroupName { get; }

    void Configure(RouteGroupBuilder groupBuilder);
}

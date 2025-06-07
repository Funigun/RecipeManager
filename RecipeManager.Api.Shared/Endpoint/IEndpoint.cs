using Microsoft.AspNetCore.Routing;

namespace RecipeManager.Api.Shared.Endpoint;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder endpoints);
}

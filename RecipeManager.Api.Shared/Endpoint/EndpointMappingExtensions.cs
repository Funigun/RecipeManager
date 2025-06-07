using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace RecipeManager.Api.Shared.Endpoint;
public static class EndpointMappingExtensions
{
    public static RouteHandlerBuilder MapBasicPost<TRequest, TResponse>
        (this IEndpointRouteBuilder builder, string pattern, Delegate handler,Action<RouteHandlerBuilder>? configureEndpoint = null)
    {
        RouteHandlerBuilder routeHandler = builder.MapPost(pattern, handler)
                                                  .Produces<TResponse>(StatusCodes.Status201Created)
                                                  .ProducesProblem(StatusCodes.Status400BadRequest)
                                                  .ProducesProblem(StatusCodes.Status404NotFound)
                                                  .ProducesProblem(StatusCodes.Status500InternalServerError);

        configureEndpoint?.Invoke(routeHandler);

        return routeHandler;
    }
}

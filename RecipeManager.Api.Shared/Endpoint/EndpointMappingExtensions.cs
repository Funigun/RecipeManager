using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using RecipeManager.Api.Shared.Filters;

namespace RecipeManager.Api.Shared.Endpoint;

public static class EndpointMappingExtensions
{
    public static RouteHandlerBuilder MapStandardPost<TRequest, TResponse>(this IEndpointRouteBuilder builder, string pattern, Delegate handler)
    {
        RouteHandlerBuilder routeHandler = builder.MapPost(pattern, handler)
                                                  .Produces<TResponse>(StatusCodes.Status201Created)
                                                  .ProducesProblem(StatusCodes.Status400BadRequest)
                                                  .ProducesProblem(StatusCodes.Status404NotFound)
                                                  .ProducesProblem(StatusCodes.Status500InternalServerError);

        return routeHandler;
    }

    public static RouteHandlerBuilder MapStandardValidatedPost<TRequest, TResponse>(this IEndpointRouteBuilder builder, string pattern, Delegate handler)
    {
        return builder.MapStandardPost<TRequest, TResponse>(pattern, handler)
                      .WithValidationFilter<TRequest>();
    }

    public static RouteHandlerBuilder MapStandardAuthenticatedPost<TRequest, TResponse>(this IEndpointRouteBuilder builder, string pattern, Delegate handler)
    {
        return builder.MapStandardPost<TRequest, TResponse>(pattern, handler)
                      .WithAuthenticationFilter<TRequest>()
                      .WithValidationFilter<TRequest>();
    }

    public static RouteHandlerBuilder WithValidationFilter<TRequest>(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter<ValidationFilter<TRequest>>();
    }

    public static RouteHandlerBuilder WithAuthenticationFilter<TRequest>(this RouteHandlerBuilder builder)
    {
        return builder.ProducesProblem(StatusCodes.Status403Forbidden)
                      .AddEndpointFilter<AuthorizationFilter<TRequest>>();
    }

    public static RouteHandlerBuilder MapStandardGet<TResponse>(this IEndpointRouteBuilder builder, string pattern, Delegate handler)
    {
        RouteHandlerBuilder routeHandler = builder.MapGet(pattern, handler)
                                                  .Produces<TResponse>(StatusCodes.Status200OK)
                                                  .ProducesProblem(StatusCodes.Status404NotFound)
                                                  .ProducesProblem(StatusCodes.Status500InternalServerError);

        return routeHandler;
    }

    public static RouteHandlerBuilder MapStandardAuthenticatedGet<TResponse>(this IEndpointRouteBuilder builder, string pattern, Delegate handler)
    {
        return builder.MapStandardGet<TResponse>(pattern, handler)
                      .WithAuthenticationFilter<TResponse>();
    }

    public static RouteHandlerBuilder MapStandardPut<TRequest>(this IEndpointRouteBuilder builder, string pattern, Delegate handler)
    {
        RouteHandlerBuilder routeHandler = builder.MapPut(pattern, handler)
                                                  .Produces(StatusCodes.Status204NoContent)
                                                  .ProducesProblem(StatusCodes.Status400BadRequest)
                                                  .ProducesProblem(StatusCodes.Status404NotFound)
                                                  .ProducesProblem(StatusCodes.Status500InternalServerError);

        return routeHandler;
    }

    public static RouteHandlerBuilder MapStandardValidatedPut<TRequest>(this IEndpointRouteBuilder builder, string pattern, Delegate handler)
    {
        return builder.MapStandardPut<TRequest>(pattern, handler)
                      .WithValidationFilter<TRequest>();
    }

    public static RouteHandlerBuilder MaptandardAuthenticatedPut<TRequest>(this IEndpointRouteBuilder builder, string pattern, Delegate handler)
    {
        return builder.MapStandardPut<TRequest>(pattern, handler)
                      .WithAuthenticationFilter<TRequest>()
                      .WithValidationFilter<TRequest>();
    }

    public static RouteHandlerBuilder MapStandardDelete<TRequest>(this IEndpointRouteBuilder builder, string pattern, Delegate handler)
    {
        RouteHandlerBuilder routeHandler = builder.MapDelete(pattern, handler)
                                                  .Produces(StatusCodes.Status204NoContent)
                                                  .ProducesProblem(StatusCodes.Status404NotFound)
                                                  .ProducesProblem(StatusCodes.Status500InternalServerError);

        return routeHandler;
    }

    public static RouteHandlerBuilder MapStandardAuthenticatedDelete<TRequest>(this IEndpointRouteBuilder builder, string pattern, Delegate handler)
    {
        return builder.MapStandardDelete<TRequest>(pattern, handler)
                      .WithAuthenticationFilter<TRequest>();
    }
}

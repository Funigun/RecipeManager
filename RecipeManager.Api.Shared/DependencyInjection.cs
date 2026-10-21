using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Api.Shared.Hateoas.Builder;
using RecipeManager.Api.Shared.Hateoas.Common;
using RecipeManager.Api.Shared.Middleware;

namespace RecipeManager.Api.Shared;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services)
    {
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IHateoasBuilderFactory, HateoasBuilder>();
        services.AddScoped<HateoasLinkService>();
        services.AddScoped<ICurrentUser, CurrentUser>();

        return services;
    }

    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services, Assembly assembly)
    {
        Type authorizationPolicyType = typeof(IAuthorizationPolicy<>);

        List<Type>? types = assembly.GetExportedTypes()
                                    .Where(t => t.GetInterfaces()
                                                     .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == authorizationPolicyType))
                                    .ToList();

        foreach (Type type in types)
        {
            Type interfaceType = type.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == authorizationPolicyType);
            services.AddScoped(interfaceType, type);
        }

        return services;
    }

    public static IApplicationBuilder UseMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<LoggingMiddleware>();
        app.UseMiddleware<PerformanceMiddleware>();

        return app;
    }

    public static WebApplication MapEndpoints(this WebApplication app, Assembly assembly)
    {
        IEnumerable<Type> endpointTypes = assembly.GetTypes()
                                                  .Where(type => typeof(IEndpoint).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface);

        foreach (Type endpointType in endpointTypes)
        {
            IEndpoint endpoint = (IEndpoint)ActivatorUtilities.CreateInstance(app.Services, endpointType);
            endpoint.MapEndpoint(app);
        }

        return app;
    }

    public static WebApplicationBuilder AddConfiguration(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                             .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

        return builder;
    }
}

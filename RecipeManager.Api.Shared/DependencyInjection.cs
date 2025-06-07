using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Api.Shared.Hateoas.Builder;
using System.Reflection;

namespace RecipeManager.Api.Shared;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services)
    {
        services.AddScoped<HateoasBuilder>();
        return services;
    }

    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        IEnumerable<Type> endpoints = assembly.GetTypes()
                                              .Where(type => typeof(IEndpoint).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface);

        foreach (Type endpointType in endpoints)
        {
            services.AddTransient(typeof(IEndpoint), endpointType);
        }

        IEnumerable<Type> groups = assembly.GetTypes()
                                           .Where(type => typeof(IGroupEndpoint).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface);

        foreach (Type group in groups)
        {
            services.AddTransient(typeof(IGroupEndpoint), group);
        }

        return services;
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        Dictionary<string, IGroupEndpoint> endpointGroups = app.Services.GetServices<IGroupEndpoint>()
                                                                        .ToDictionary(g => g.Name);

        List<IEndpoint> endpoints = app.Services.GetServices<IEndpoint>().ToList();

        Dictionary<string, List<IEndpoint>> groupedEndpoints = [];
        List<IEndpoint> ungroupedEndpoints = [];

        foreach (IEndpoint endpoint in endpoints)
        {
            GroupEndpointAttribute? attribute = endpoint.GetType().GetCustomAttribute<GroupEndpointAttribute>();

            if (attribute != null)
            {
                if (!groupedEndpoints.ContainsKey(attribute.GroupName))
                {
                    groupedEndpoints[attribute.GroupName] = [];
                }

                groupedEndpoints[attribute.GroupName].Add(endpoint);
            }
            else
            {
                ungroupedEndpoints.Add(endpoint);
            }
        }

        foreach (KeyValuePair<string, List<IEndpoint>> group in groupedEndpoints)
        {
            string groupName = group.Key;
            List<IEndpoint> groupEndpoints = group.Value;

            string groupRoute = $"api/{groupName.ToLowerInvariant()}";
            RouteGroupBuilder routeGroupBuilder = app.MapGroup(groupRoute);

            if (endpointGroups.TryGetValue(groupName, out IGroupEndpoint? endpointGroup))
            {
                endpointGroup.Configure(routeGroupBuilder);
            }

            foreach (IEndpoint endpoint in groupEndpoints)
            {
                endpoint.MapEndpoint(routeGroupBuilder);
            }
        }

        foreach (IEndpoint endpoint in ungroupedEndpoints)
        {
            endpoint.MapEndpoint(app);
        }

        return app;
    }
}

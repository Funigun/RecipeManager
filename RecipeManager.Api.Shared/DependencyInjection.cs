using Microsoft.Extensions.DependencyInjection;
using RecipeManager.Api.Shared.Hateoas.Builder;

namespace RecipeManager.Api.Shared;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services)
    {
        services.AddScoped<HateoasBuilder>();
        return services;
    }
}

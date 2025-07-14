using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;

namespace RecipeManager.Api.Persistance;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistance(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("RecipeManager"));
        });

        services.AddScoped<IAppDbContext, AppDbContext>();

        return services;
    }
}

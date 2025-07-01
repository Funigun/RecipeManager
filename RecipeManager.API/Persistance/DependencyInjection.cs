using Microsoft.EntityFrameworkCore;
using RecipeManager.API.Application.Abstractions;

namespace RecipeManager.API.Persistance;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistance(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseInMemoryDatabase("RecipeManagerDb");
            //options.UseSqlServer(configuration.GetConnectionString("RecipeManager"));
        });

        services.AddScoped<IAppDbContext, AppDbContext>();

        return services;
    }
}

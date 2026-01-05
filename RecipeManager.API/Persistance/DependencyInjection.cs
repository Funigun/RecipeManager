using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Application.Database;
using RecipeManager.Api.Application.Database.Repositories;
using RecipeManager.Api.Persistance.Repositories;

namespace RecipeManager.Api.Persistance;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistance(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("RecipeManager"), o => o.UseCompatibilityLevel(170));
        });

        services.AddScoped<IAppDbContext, AppDbContext>();
        services.AddScoped<IUnitRepository, UnitRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}

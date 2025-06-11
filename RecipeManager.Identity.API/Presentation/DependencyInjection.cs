using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecipeManager.Identity.API.Domain;
using RecipeManager.Identity.API.Persistance;
using Scalar.AspNetCore;
using System.Text;

namespace RecipeManager.Identity.API.Presentation;

internal static class DependencyInjection
{
    internal static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)  
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["JwtSettings:Key"]!)),
                        ValidIssuer = configuration["JwtSettings:Issuer"]!,
                        ValidAudience = configuration["JwtSettings:Audience"]!,
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                    };
                });

        return services;
    }

    internal static IServiceCollection ConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentityCore<User>()
                .AddRoles<Role>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddApiEndpoints();

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 10;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredUniqueChars = 3;
            options.User.RequireUniqueEmail = true;
        });

        return services;
    }

    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("RecipeManager"));
        });

        services.AddScoped<DatabaseService>();

        return services;
    }

    public static async Task InitDatabase(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        DatabaseService dbService = scope.ServiceProvider.GetRequiredService<DatabaseService>();
        await dbService.InitDatabase();
    }

    public static ScalarOptions ConfigureScalarOptions(this ScalarOptions options)
    {
        options.WithTitle("Recipe Manager Identity API")
               .WithTheme(ScalarTheme.Kepler)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);

        options.AddPreferredSecuritySchemes("Bearer");
        options.AddHttpAuthentication("Bearer", o => o.Description = "Provide valid token");

        options.Servers = [new("https://localhost:7002")];

        return options;
    }
}

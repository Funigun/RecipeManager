using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecipeManager.Api;
using RecipeManager.Api.Persistance;
using RecipeManager.Integration.Tests.Common.Users;

namespace RecipeManager.Integration.Tests.CoreApi.TestFixtures;

public class WebApiFactory(string connectionString) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            ServiceDescriptor? descriptor = services.SingleOrDefault(descriptor => descriptor.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Configuration = new() { Issuer = TokenMockFactory.Issuer };
                options.TokenValidationParameters.ValidIssuer = TokenMockFactory.Issuer;
                options.TokenValidationParameters.ValidAudience = TokenMockFactory.Audience;
                options.Configuration.SigningKeys.Add(TokenMockFactory.SecurityKey);
            });
        })
        .UseEnvironment("Development");
    }
}

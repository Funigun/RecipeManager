using System.Net.Http.Headers;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecipeManager.Api.Persistance;
using RecipeManager.Api.Presentation;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;
using Testcontainers.MsSql;

[assembly: AssemblyFixture(typeof(WebApiFactory))]

namespace RecipeManager.Integration.Tests.CoreApi.TestFixtures;

public sealed class WebApiFactory : WebApplicationFactory<IAssemblyMarker>, IAsyncLifetime
{
    private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder().WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                                                                      .WithPassword("Str0ng_P@ssw0rd4Tests")
                                                                      .WithPortBinding(1433)
                                                                      .WithEnvironment("ACCEPT_EULA", "Y")
                                                                      .WithName("MealsManagerTestDb")
                                                                      .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
                                                                      .Build();

    public AppDbContext DbContext { get; private set; } = default!;

    public HttpClient HttpClient { get; private set; } = default!;

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
            {
                options.UseSqlServer(_sqlContainer.GetConnectionString());
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            });

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

    public string ConnectionString => _sqlContainer.GetConnectionString();

    async ValueTask IAsyncLifetime.InitializeAsync()
    {
        await _sqlContainer.StartAsync();

        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                                                 .UseSqlServer(_sqlContainer.GetConnectionString())
                                                 .Options;

        DbContext = new(options, UserMockFactory.CreateMockedAdmin());

        await DbContext.Database.MigrateAsync();

        HttpClient = CreateClient();

        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenMockFactory.GenerateJwtToken(UserMockFactory.CreateMockedAdmin()));
        _ = await HttpClient.GetAsync("/api/units", TestContext.Current.CancellationToken);

        await DbContext.SeedAsync();
    }

    public new async Task DisposeAsync()
    {
        await DbContext.DisposeAsync();
        await _sqlContainer.StopAsync();
        await _sqlContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}

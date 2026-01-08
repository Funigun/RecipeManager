using System.Net.Http.Headers;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Images;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RecipeManager.Api.Persistance;
using RecipeManager.Api.Presentation;
using RecipeManager.Integration.Tests.Common.Users;
using RecipeManager.Integration.Tests.CoreApi.TestFixtures;
using StackExchange.Redis;
using Testcontainers.MsSql;
using Testcontainers.Redis;

[assembly: AssemblyFixture(typeof(WebApiFactory))]

namespace RecipeManager.Integration.Tests.CoreApi.TestFixtures;

public sealed class WebApiFactory : WebApplicationFactory<IAssemblyMarker>, IAsyncLifetime
{
    private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2025-latest")
                                                                      .WithPassword("Str0ng_P@ssw0rd4Tests")
                                                                      .WithPortBinding(1433)
                                                                      .WithEnvironment("ACCEPT_EULA", "Y")
                                                                      .WithName("RecipesManagerTestDb")
                                                                      .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(1433))
                                                                      .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder("redis:7.0").Build();

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
                options.UseSqlServer(_sqlContainer.GetConnectionString(), o => o.UseCompatibilityLevel(170));
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            });

            services.RemoveAll(typeof(RedisCacheOptions));
            services.RemoveAll(typeof(IConnectionMultiplexer));
            services.RemoveAll(typeof(HybridCacheEntryOptions));
            services.RemoveAll(typeof(HybridCache));

            IConnectionMultiplexer redis = ConnectionMultiplexer.Connect(_redisContainer.GetConnectionString());
            services.AddSingleton<IConnectionMultiplexer>(redis);
            services.AddStackExchangeRedisCache(opt => opt.ConnectionMultiplexerFactory = () => Task.FromResult(redis));

            services.AddHybridCache(options => options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Flags = HybridCacheEntryFlags.DisableLocalCache
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
        await _redisContainer.StartAsync();

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

        await _redisContainer.StopAsync();
        await _redisContainer.DisposeAsync();

        await base.DisposeAsync();
    }
}

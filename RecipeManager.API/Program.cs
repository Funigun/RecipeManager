using System.Reflection;
using System.Text.Json.Serialization.Metadata;
using FluentValidation;
using Microsoft.Extensions.Caching.Hybrid;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Persistance;
using RecipeManager.Api.Persistance.Cache;
using RecipeManager.Api.Presentation;
using RecipeManager.Api.Features.Units.Command.CreateUnit;
using RecipeManager.Api.Features.Units.Command.DeleteUnit;
using RecipeManager.Api.Features.Units.Command.UpdateUnit;
using RecipeManager.Api.Features.Units.Query.GetAllUnits;
using RecipeManager.Api.Features.Units.Query.GetPrimaryUnits;
using RecipeManager.Api.Features.Units.Query.GetUnitById;
using RecipeManager.Api.Features.Units.Query.GetUnitsForDropdown;
using RecipeManager.Api.Shared;
using RecipeManager.ServiceDefaults;
using Scalar.AspNetCore;
using Serilog;
using StackExchange.Redis;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Assembly assembly = Assembly.GetExecutingAssembly();

Log.Logger = new LoggerConfiguration().ReadFrom
             .Configuration(new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .Build())
             .CreateLogger();

try
{
    Log.Information("Starting Recipe Manager API");

    builder.AddConfiguration();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("RecipeManagerCorsPolicy", policy =>
        {
            policy.WithOrigins("https://localhost:7000")
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    builder.Services.AddSerilog();
    builder.AddServiceDefaults("recipe-manager-api");

    builder.Services.AddOpenApi("v1", options =>
    {
        options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_1;
        options.CreateSchemaReferenceId = (JsonTypeInfo typeInfo) =>
        {
            return typeInfo.Type.FullName;
        };
    });

    builder.Services.AddSharedServices()
                    .AddAuthorizationPolicies(assembly)
                    .AddValidatorsFromAssembly(assembly);

    builder.Services.AddScoped<CreateUnitCommandHandler>();
    builder.Services.AddScoped<DeleteUnitCommandHandler>();
    builder.Services.AddScoped<UpdateUnitCommandHandler>();
    builder.Services.AddScoped<GetAllUnitsQueryHandler>();
    builder.Services.AddScoped<GetPrimaryUnitsQueryHandler>();
    builder.Services.AddScoped<GetUnitByIdQueryHandler>();
    builder.Services.AddScoped<GetUnitsForDropdownQueryHandler>();

    builder.Services.AddPersistance(builder.Configuration);

    builder.AddRedisClient("Cache");
    IConnectionMultiplexer? redis = null;

    try
    {
        redis = builder.Services.BuildServiceProvider().GetService<IConnectionMultiplexer>();
    }
    catch(Exception e)
    {

    }

    if (redis is not null)
    {
        builder.Services.AddStackExchangeRedisCache(opt => opt.ConnectionMultiplexerFactory = () => Task.FromResult(redis));
        builder.Services.AddHybridCache(options => options.DefaultEntryOptions = new HybridCacheEntryOptions
        {
            Flags = HybridCacheEntryFlags.DisableLocalCache
        });
    }
    else
    {
        builder.Services.AddHybridCache();
    }

    builder.Services.AddSingleton<ICacheService, HybridCacheService>();

    builder.Services.ConfigureAuthentication(builder.Configuration)
                    .AddAuthorizationBuilder()
                    .AddPolicy("RecipeManagerPolicy", policy =>
                    {
                        policy.RequireAuthenticatedUser();
                    });

    WebApplication app = builder.Build();

    app.UseMiddlewares();
    app.UseRouting();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options => options.ConfigureScalarOptions());
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseSerilogRequestLogging();
    app.UseCors("RecipeManagerCorsPolicy");
    app.MapDefaultEndpoints();
    app.MapEndpoints(assembly);
    app.UseAuthorization();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "API startup failed");
}
finally
{
    await Log.CloseAndFlushAsync();
}

namespace RecipeManager.Api
{
    public abstract partial class Program
    {
    }
}

using System.Reflection;
using System.Text.Json.Serialization.Metadata;
using FluentValidation;
using RecipeManager.Api.Persistance;
using RecipeManager.Api.Presentation;
using RecipeManager.Api.Shared;
using RecipeManager.ServiceDefaults;
using Scalar.AspNetCore;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Assembly assembly = Assembly.GetExecutingAssembly();
Assembly contractsAssembly = Assembly.GetAssembly(typeof(RecipeManager.Shared.Contracts.AssemblyReader))!;

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
                    .AddEndpoints(assembly)
                    .AddAuthorizationPolicies(assembly)
                    .AddValidatorsFromAssembly(assembly)
                    .AddValidatorsFromAssembly(contractsAssembly);
                    //.AddScoped<IRedisService, RedisService>();

    builder.Services.AddPersistance(builder.Configuration);

    builder.Services.ConfigureAuthentication(builder.Configuration)
                    .AddAuthorizationBuilder()
                    .AddPolicy("RecipeManagerPolicy", policy =>
                    {
                        policy.RequireAuthenticatedUser();
                    });

    //builder.AddRedisClient("Cache");

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
    app.MapEndpoints();
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

using System.Reflection;
using FluentValidation;
using RecipeManager.Api.Shared;
using RecipeManager.Identity.API.Presentation;
using RecipeManager.ServiceDefaults;
using Scalar.AspNetCore;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Assembly apiAssembly = Assembly.GetExecutingAssembly();
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
    Log.Information("Starting Recipe Manager Identity API");

    builder.AddConfiguration();
    builder.Services.AddSerilog();
    builder.AddServiceDefaults("recipe-manager-identity-api");

    builder.Services.AddOpenApi("v1", options =>
    {
        options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    });

    builder.Services.AddSharedServices()
                    .AddEndpoints(apiAssembly)
                    .AddAuthorizationPolicies(apiAssembly)
                    .AddValidatorsFromAssembly(apiAssembly)
                    .AddValidatorsFromAssembly(contractsAssembly);

    builder.Services.ConfigureIdentity()
                    .ConfigureDatabase(builder.Configuration);

    builder.Services.ConfigureAuthentication(builder.Configuration)
                    .AddAuthorizationBuilder()
                    .AddPolicy("RecipeManagerPolicy", policy =>
                    {
                        policy.RequireAuthenticatedUser();
                    });

    WebApplication app = builder.Build();

    app.MapDefaultEndpoints();

    app.UseMiddlewares();
    app.UseRouting();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options => options.ConfigureScalarOptions());
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.MapEndpoints();
    app.UseAuthorization();

    await app.InitDatabase();
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Identity API startup failed");
}
finally
{
    await Log.CloseAndFlushAsync();
}

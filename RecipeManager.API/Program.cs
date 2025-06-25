using System.Reflection;
using RecipeManager.Api.Shared;
using RecipeManager.ServiceDefaults;
using Scalar.AspNetCore;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().ReadFrom
             .Configuration(new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .Build())
             .CreateLogger();

Assembly assembly = Assembly.GetExecutingAssembly();

try
{
    Log.Information("Starting Recipe Manager API");

    builder.AddConfiguration();
    builder.Services.AddSerilog();

    builder.AddServiceDefaults("recipe-manager-api");

    builder.Services.AddOpenApi();

    builder.Services.AddSharedServices()
                    .AddEndpoints(assembly)
                    .AddAuthorizationPolicies(assembly);

    WebApplication app = builder.Build();

    app.MapDefaultEndpoints();

    app.UseMiddlewares();
    app.UseRouting();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.WithTitle("Recipe Manager API")
                       .WithTheme(ScalarTheme.DeepSpace)
                       .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);

            options.Servers = [new("https://localhost:7001")];
        });
    }

    app.UseHttpsRedirection();

    app.MapEndpoints();

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

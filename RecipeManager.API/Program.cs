using RecipeManager.Api.Shared;
using RecipeManager.ServiceDefaults;
using Scalar.AspNetCore;
using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.Services.AddSharedServices()
                .AddEndpoints(Assembly.GetExecutingAssembly());

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

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

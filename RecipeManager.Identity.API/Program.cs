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

app.UseMiddlewares();
app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Recipe Manager Identity API")
                   .WithTheme(ScalarTheme.Kepler)
                   .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);

        options.Servers = [new("https://localhost:7002")];
    });
}

app.UseHttpsRedirection();

app.MapEndpoints();

await app.RunAsync();
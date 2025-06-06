using RecipeManager.ServiceDefaults;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

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

await app.RunAsync();
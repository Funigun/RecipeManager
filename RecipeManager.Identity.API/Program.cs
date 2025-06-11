using RecipeManager.Api.Shared;
using RecipeManager.Identity.API.Presentation;
using RecipeManager.ServiceDefaults;
using Scalar.AspNetCore;
using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
Assembly assembly = Assembly.GetExecutingAssembly();

builder.AddServiceDefaults();

builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

builder.Services.AddSharedServices()
                .AddEndpoints(assembly)
                .AddAuthorizationPolicies(assembly);

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
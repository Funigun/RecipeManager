using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using RecipeManager.ServiceDefaults;
using RecipeManager.UI.Blazor.Components;
using RecipeManager.UI.Blazor.Services;
using RecipeManager.UI.Blazor.Services.Authorization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

builder.AddServiceDefaults();

builder.Services.AddMudServices();

builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents();

builder.Services.AddAuthenticationCore()
                .AddCascadingAuthenticationState()
                .AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

builder.Services.AddHttpClient<IdentityApiService>(option =>
{
    option.BaseAddress = new Uri(builder.Configuration["IdentityApi:BaseUrl"]!);
});

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode()
   .AddInteractiveWebAssemblyRenderMode()
   .AddAdditionalAssemblies(typeof(RecipeManager.UI.Blazor.Client._Imports).Assembly);

await app.RunAsync();
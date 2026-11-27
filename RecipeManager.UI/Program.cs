using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using OpenTelemetry.Logs;
using RecipeManager.UI;
using RecipeManager.UI.Features.Account.Login;
using RecipeManager.UI.Features.Account.Register;
using RecipeManager.UI.Features.Account.Shared;
using RecipeManager.UI.Shared.Appearance;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

Uri appBaseAddress = new(builder.HostEnvironment.BaseAddress);
Uri identityApiBaseAddress = new(appBaseAddress, "_api/recipe-manager-identity-api/");
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = identityApiBaseAddress });
builder.Services.AddScoped<SignInApiClient>();
builder.Services.AddScoped<RegistrationApiClient>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<UiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(services =>
    services.GetRequiredService<UiAuthenticationStateProvider>());
builder.Services.AddMudServices();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddScoped<ThemeService>();

if (!string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]))
{
    builder.Logging.AddOpenTelemetry(logging =>
    {
        logging.IncludeFormattedMessage = true;
        logging.IncludeScopes = true;
        logging.AddOtlpExporter();
    });
}

await builder.Build().RunAsync();

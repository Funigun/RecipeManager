using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using MudBlazor.Services;
using RecipeManager.ServiceDefaults;
using RecipeManager.UI.Blazor.Brokers.IdentityApi;
using RecipeManager.UI.Blazor.Brokers.RecipeManagersApi;
using RecipeManager.UI.Blazor.Components;
using RecipeManager.UI.Blazor.Components.Common.DialogMessage;
using RecipeManager.UI.Blazor.Features.IngredientCategories.Services;
using RecipeManager.UI.Blazor.Features.Ingredients.Services;
using RecipeManager.UI.Blazor.Features.RecipeCategories.Services;
using RecipeManager.UI.Blazor.Features.Recipes.Services;
using RecipeManager.UI.Blazor.Features.Units.Services;
using RecipeManager.UI.Blazor.Services.Authentication;
using RecipeManager.UI.Blazor.Services.Authorization;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

Log.Logger = new LoggerConfiguration().ReadFrom
                                      .Configuration(new ConfigurationBuilder()
                                      .SetBasePath(Directory.GetCurrentDirectory())
                                      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                      .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                                      .Build())
                                      .CreateLogger();

try
{
    Log.Information("Starting Recipe Manager Blazor UI");

    builder.Services.AddSerilog();
    builder.AddServiceDefaults("recipe-manager-ui");

    builder.Services.AddMudServices(config =>
    {
        config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;

        config.SnackbarConfiguration.PreventDuplicates = false;
        config.SnackbarConfiguration.NewestOnTop = false;
        config.SnackbarConfiguration.ShowCloseIcon = true;
        config.SnackbarConfiguration.VisibleStateDuration = 3500;
        config.SnackbarConfiguration.HideTransitionDuration = 500;
        config.SnackbarConfiguration.ShowTransitionDuration = 500;
        config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
    })
    .AddMudMarkdownServices();

    builder.Services.AddRazorComponents()
                    .AddInteractiveServerComponents()
                    .AddInteractiveWebAssemblyComponents();

    builder.Services.AddAuthenticationCore()
                    .AddCascadingAuthenticationState()
                    .AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

    builder.Services.AddHttpClient<IIdentityApi, IdentityApi>(option =>
    {
        option.BaseAddress = new Uri(builder.Configuration["IdentityApi:BaseUrl"]!);
    });

    builder.Services.AddHttpClient<IRecipeApi, RecipeApi>(option =>
    {
        option.BaseAddress = new Uri(builder.Configuration["RecipesApi:BaseUrl"]!);
    });

    builder.Services.AddScoped<IAuthenticationService, AuthenticationService>()
                    .AddScoped<IDialogMessageService, DialogMessageService>()
                    .AddScoped<IUnitService, UnitService>()
                    .AddScoped<IIngredientCategoryService, IngredientCategoryService>()
                    .AddScoped<IRecipeCategoryService, RecipeCategoryService>()
                    .AddScoped<IIngredientService, IngredientService>()
                    .AddScoped<IRecipeService, RecipeService>();

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
}
catch (Exception ex)
{
    Log.Fatal(ex, "An error occurred while starting the Recipe Manager UI");
}
finally
{
    await Log.CloseAndFlushAsync();
}

using System.Reflection;
using System.Text.Json.Serialization.Metadata;
using FluentValidation;
using Microsoft.Extensions.Caching.Hybrid;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Persistance;
using RecipeManager.Api.Persistance.Cache;
using RecipeManager.Api.Presentation;
using RecipeManager.Api.Features.IngredientCategories.Command.CreateIngredientCategory;
using RecipeManager.Api.Features.IngredientCategories.Command.DeleteIngredientCategory;
using RecipeManager.Api.Features.IngredientCategories.Query.GetIngredientCategories;
using RecipeManager.Api.Features.IngredientCategories.Query.GetIngredientCategoriesForDropdown;
using RecipeManager.Api.Features.Ingredients.Command.CreateIngredient;
using RecipeManager.Api.Features.Ingredients.Command.DeleteIngredient;
using RecipeManager.Api.Features.Ingredients.Command.GenerateShoppingList;
using RecipeManager.Api.Features.Ingredients.Command.UpdateIngredient;
using RecipeManager.Api.Features.Ingredients.Query.GetIngredientById;
using RecipeManager.Api.Features.Ingredients.Query.GetIngredients;
using RecipeManager.Api.Features.Ingredients.Query.GetIngredientsForDropdown;
using RecipeManager.Api.Features.MealPlanner.Command.UpdateMealPlan;
using RecipeManager.Api.Features.MealPlanner.Query.GetMealPlan;
using RecipeManager.Api.Features.RecipeCategories.Command.CreateRecipeCategory;
using RecipeManager.Api.Features.RecipeCategories.Command.DeleteRecipeCategory;
using RecipeManager.Api.Features.RecipeCategories.Query.GetRecipeCategories;
using RecipeManager.Api.Features.RecipeCategories.Query.GetRecipeCategoriesForDropdown;
using RecipeManager.Api.Features.RecipeCategories.Query.GetRecipeCategoriesForFiltering;
using RecipeManager.Api.Features.Recipes.Command.CreateRecipe;
using RecipeManager.Api.Features.Recipes.Command.DeleteRecipe;
using RecipeManager.Api.Features.Recipes.Command.UpdateRecipe;
using RecipeManager.Api.Features.Recipes.Query.GetRecipeById;
using RecipeManager.Api.Features.Recipes.Query.GetRecipeForDropdownModel;
using RecipeManager.Api.Features.Recipes.Query.GetRecipes;
using RecipeManager.Api.Features.Recipes.Query.GetRecipesForMealPlan;
using RecipeManager.Api.Features.Cookbooks.Command.CreateCookbook;
using RecipeManager.Api.Features.Cookbooks.Command.DeleteCookbook;
using RecipeManager.Api.Features.Cookbooks.Command.UpdateCookbook;
using RecipeManager.Api.Features.Cookbooks.Query.GetCookbookById;
using RecipeManager.Api.Features.Cookbooks.Query.GetCookbooks;
using RecipeManager.Api.Features.Units.Command.CreateUnit;
using RecipeManager.Api.Features.Units.Command.DeleteUnit;
using RecipeManager.Api.Features.Units.Command.UpdateUnit;
using RecipeManager.Api.Features.Units.Query.GetAllUnits;
using RecipeManager.Api.Features.Units.Query.GetPrimaryUnits;
using RecipeManager.Api.Features.Units.Query.GetUnitById;
using RecipeManager.Api.Features.Units.Query.GetUnitsForDropdown;
using RecipeManager.Api.Shared;
using RecipeManager.ServiceDefaults;
using Scalar.AspNetCore;
using Serilog;
using StackExchange.Redis;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Assembly assembly = Assembly.GetExecutingAssembly();

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
                    .AddAuthorizationPolicies(assembly)
                    .AddValidatorsFromAssembly(assembly);

    builder.Services.AddScoped<CreateUnitCommandHandler>();
    builder.Services.AddScoped<DeleteUnitCommandHandler>();
    builder.Services.AddScoped<UpdateUnitCommandHandler>();
    builder.Services.AddScoped<CreateIngredientCategoryCommandHandler>();
    builder.Services.AddScoped<DeleteIngredientCategoryCommandHandler>();
    builder.Services.AddScoped<GetIngredientCategoriesQueryHandler>();
    builder.Services.AddScoped<GetIngredientCategoriesForDropdownQueryHandler>();
    builder.Services.AddScoped<CreateIngredientCommandHandler>();
    builder.Services.AddScoped<DeleteIngredientCommandHandler>();
    builder.Services.AddScoped<GenerateShoppingListCommandHandler>();
    builder.Services.AddScoped<UpdateIngredientCommandHandler>();
    builder.Services.AddScoped<GetIngredientByIdQueryHandler>();
    builder.Services.AddScoped<GetIngredientsQueryHandler>();
    builder.Services.AddScoped<GetIngredientsForDropdownQueryHandler>();
    builder.Services.AddScoped<UpdateMealPlanCommandHandler>();
    builder.Services.AddScoped<GetMealPlanQueryHandler>();
    builder.Services.AddScoped<CreateRecipeCategoryCommandHandler>();
    builder.Services.AddScoped<DeleteRecipeCategoryCommandHandler>();
    builder.Services.AddScoped<GetRecipeCategoriesQueryHandler>();
    builder.Services.AddScoped<GetRecipeCategoriesForDropdownQueryHandler>();
    builder.Services.AddScoped<GetRecipeCategoriesForFilteringQueryHandler>();
    builder.Services.AddScoped<CreateRecipeCommandHandler>();
    builder.Services.AddScoped<DeleteRecipeCommandHandler>();
    builder.Services.AddScoped<UpdateRecipeCommandHandler>();
    builder.Services.AddScoped<GetRecipeByIdQueryHandler>();
    builder.Services.AddScoped<GetRecipeForDropdownModelQueryHandler>();
    builder.Services.AddScoped<GetRecipesQueryHandler>();
    builder.Services.AddScoped<GetRecipesForMealPlanQueryHandler>();
    builder.Services.AddScoped<CreateCookbookCommandHandler>();
    builder.Services.AddScoped<DeleteCookbookCommandHandler>();
    builder.Services.AddScoped<UpdateCookbookCommandHandler>();
    builder.Services.AddScoped<GetCookbookByIdQueryHandler>();
    builder.Services.AddScoped<GetCookbooksQueryHandler>();
    builder.Services.AddScoped<GetAllUnitsQueryHandler>();
    builder.Services.AddScoped<GetPrimaryUnitsQueryHandler>();
    builder.Services.AddScoped<GetUnitByIdQueryHandler>();
    builder.Services.AddScoped<GetUnitsForDropdownQueryHandler>();

    builder.Services.AddPersistance(builder.Configuration);

    builder.AddRedisClient("Cache");
    IConnectionMultiplexer? redis = null;

    try
    {
        redis = builder.Services.BuildServiceProvider().GetService<IConnectionMultiplexer>();
    }
    catch(Exception e)
    {

    }

    if (redis is not null)
    {
        builder.Services.AddStackExchangeRedisCache(opt => opt.ConnectionMultiplexerFactory = () => Task.FromResult(redis));
        builder.Services.AddHybridCache(options => options.DefaultEntryOptions = new HybridCacheEntryOptions
        {
            Flags = HybridCacheEntryFlags.DisableLocalCache
        });
    }
    else
    {
        builder.Services.AddHybridCache();
    }

    builder.Services.AddSingleton<ICacheService, HybridCacheService>();

    builder.Services.ConfigureAuthentication(builder.Configuration)
                    .AddAuthorizationBuilder()
                    .AddPolicy("RecipeManagerPolicy", policy =>
                    {
                        policy.RequireAuthenticatedUser();
                    });

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
    app.MapEndpoints(assembly);
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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RecipeManager.API.Application.Abstractions;
using RecipeManager.API.Persistance;
using RecipeManager.Api.Shared;

namespace RecipeManager.Tests.ArchitectureTests.MainApi.Fixtures;

public sealed class WebApplicationFixture
{
    public WebApplicationBuilder Builder { get; }

    public WebApplication App { get; }

    public EndpointDataSource DataSource { get; }

    public WebApplicationFixture()
    {
        System.Reflection.Assembly assembly = typeof(IAppDbContext).Assembly;

        Builder = WebApplication.CreateBuilder();
        Builder.Services.AddEndpoints(assembly);
        Builder.Services.AddPersistance(Builder.Configuration);

        App = Builder.Build();
        App.UseRouting();
        App.MapEndpoints();
        App.Start();

        DataSource = App.Services.GetRequiredService<EndpointDataSource>();
    }
}

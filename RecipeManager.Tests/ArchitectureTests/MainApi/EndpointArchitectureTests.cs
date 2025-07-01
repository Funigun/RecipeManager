using System.Reflection;
using System.Reflection.PortableExecutable;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RecipeManager.Api.Shared;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.API.Application.Abstractions;
using RecipeManager.API.Persistance;

namespace RecipeManager.Tests.ArchitectureTests.MainApi;

[Trait("Core.Api.Endpoints", TestCategories.ArchitectureTests)]
public sealed class EndpointArchitectureTests
{
    private readonly Assembly _assembly = typeof(IAppDbContext).Assembly;

    [Fact]
    public void All_ValidatedPostEndpoints_ShouldHave_NestedValidator()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Services.AddEndpoints(_assembly);
        builder.Services.AddPersistance(builder.Configuration);

        WebApplication app = builder.Build();
        app.UseRouting();
        app.MapEndpoints();
        app.Start();

        EndpointDataSource dataSource = app.Services.GetRequiredService<EndpointDataSource>();
        IEnumerable<RouteEndpoint> endpoints = dataSource.Endpoints.OfType<RouteEndpoint>();

        foreach (RouteEndpoint endpoint in endpoints)
        {
            if (endpoint.Metadata.OfType<string>().Any(data => data == "Uses validation filter"))
            {
                Type declaredEndpoint = endpoint.Metadata.OfType<Type>().FirstOrDefault(t => typeof(IEndpoint).IsAssignableFrom(t))!;
                Type endpointParent = declaredEndpoint.DeclaringType!;

                Type? request = endpointParent.GetNestedType("Request", BindingFlags.Public | BindingFlags.NonPublic) ?? null;

                if (request == null)
                {
                    continue;
                }

                Type? validatorType = endpointParent.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
                                                    .FirstOrDefault(t => typeof(IValidator<>).MakeGenericType(request).IsAssignableFrom(t));

                Assert.NotNull(validatorType);
            }
        }


        Assert.Equal(1, 1);
    }
}

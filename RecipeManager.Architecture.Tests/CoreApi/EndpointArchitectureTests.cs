using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Routing;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Architecture.Tests.CoreApi.TestFixtures;

namespace RecipeManager.Architecture.Tests.CoreApi;

[Trait("Core.Api", "Endpoints")]
public sealed class EndpointArchitectureTests : IClassFixture<WebApplicationFixture>
{
    private readonly WebApplicationFixture _webApplicationFixture;

    public EndpointArchitectureTests(WebApplicationFixture webApplicationFixture)
    {
        _webApplicationFixture = webApplicationFixture;
    }

    [Fact]
    public void All_ValidatedPostEndpoints_ShouldHave_NestedValidator()
    {
        IEnumerable<RouteEndpoint> endpoints = _webApplicationFixture.DataSource.Endpoints.OfType<RouteEndpoint>();

        foreach (RouteEndpoint endpoint in endpoints)
        {
            if (endpoint.Metadata.OfType<string>().Any(data => data == "Uses validation filter"))
            {
                Type endpointParent = endpoint.Metadata.OfType<Type>().FirstOrDefault(t => t.GetNestedTypes().Any(tt => typeof(IEndpoint).IsAssignableFrom(tt)))!;

                Type? request = endpointParent.GetNestedType("Request", BindingFlags.Public | BindingFlags.NonPublic) ?? null;

                if (request == null)
                {
                    continue;
                }

                Type? validatorType = endpointParent.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
                                                    .FirstOrDefault(t => typeof(IValidator<>).MakeGenericType(request).IsAssignableFrom(t));

                Assert.True(validatorType != null, $"Validator for {endpointParent.FullName} is missing or is in wrong place");
            }
        }
    }

    [Fact]
    public void All_AuthenticatedPostEndpoints_ShouldHave_NestedAuhorizationPolicy()
    {
        IEnumerable<RouteEndpoint> endpoints = _webApplicationFixture.DataSource.Endpoints.OfType<RouteEndpoint>();

        foreach (RouteEndpoint endpoint in endpoints)
        {
            if (endpoint.Metadata.OfType<string>().Any(data => data == "Uses authentication filter"))
            {
                Type endpointParent = endpoint.Metadata.OfType<Type>().FirstOrDefault(t => t.GetNestedTypes().Any(tt => typeof(IEndpoint).IsAssignableFrom(tt)))!;

                Type? request = endpointParent.GetNestedType("Request", BindingFlags.Public | BindingFlags.NonPublic) ?? null;

                if (request == null)
                {
                    continue;
                }

                Type? validatorType = endpointParent.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
                                                    .FirstOrDefault(t => typeof(IAuthorizationPolicy<>).MakeGenericType(request).IsAssignableFrom(t));

                Assert.True(validatorType != null, $"Authorization Policy for {endpointParent.FullName} is missing or is in wrong place");
            }
        }
    }
}

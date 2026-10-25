using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Routing;
using RecipeManager.Api.Shared.Contracts.Authorization;
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
    public void All_ValidatedPostEndpoints_ShouldHave_FeatureValidator()
    {
        IEnumerable<RouteEndpoint> endpoints = _webApplicationFixture.DataSource.Endpoints.OfType<RouteEndpoint>();

        foreach (RouteEndpoint endpoint in endpoints)
        {
            if (endpoint.Metadata.OfType<string>().Any(data => data == "Uses validation filter"))
            {
                Type? request = GetRequestType(endpoint);

                if (request == null)
                {
                    continue;
                }

                IEnumerable<Type> featureTypes = GetFeatureTypes(request);
                Type? validatorType = featureTypes
                    .FirstOrDefault(t => typeof(IValidator<>).MakeGenericType(request).IsAssignableFrom(t))
                    ?? featureTypes
                    .FirstOrDefault(t => t.GetInterfaces()
                        .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>)));

                Assert.True(validatorType != null, $"Validator for {request.FullName} is missing or is in the wrong feature");
            }
        }
    }

    [Fact]
    public void All_AuthenticatedPostEndpoints_ShouldHave_FeatureAuthorizationPolicy()
    {
        IEnumerable<RouteEndpoint> endpoints = _webApplicationFixture.DataSource.Endpoints.OfType<RouteEndpoint>();

        foreach (RouteEndpoint endpoint in endpoints)
        {
            if (endpoint.Metadata.OfType<string>().Any(data => data == "Uses authentication filter"))
            {
                Type? request = GetRequestType(endpoint);

                if (request == null)
                {
                    continue;
                }

                Type? policyType = GetFeatureTypes(request)
                    .FirstOrDefault(t => typeof(IAuthorizationPolicy<>).MakeGenericType(request).IsAssignableFrom(t));

                Assert.True(policyType != null, $"Authorization policy for {request.FullName} is missing or is in the wrong feature");
            }
        }
    }

    private static Type? GetRequestType(RouteEndpoint endpoint)
    {
        Type? metadataType = endpoint.Metadata.OfType<Type>().FirstOrDefault();

        return metadataType?.GetNestedType("Request", BindingFlags.Public | BindingFlags.NonPublic)
            ?? metadataType?.DeclaringType?.GetNestedType("Request", BindingFlags.Public | BindingFlags.NonPublic)
            ?? metadataType;
    }

    private static IEnumerable<Type> GetFeatureTypes(Type request)
    {
        if (request.DeclaringType is Type featureContainer)
        {
            return featureContainer.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
                                  .Append(featureContainer);
        }

        return request.Assembly.GetTypes()
                      .Where(type => !type.IsNested && type.Namespace == request.Namespace);
    }
}

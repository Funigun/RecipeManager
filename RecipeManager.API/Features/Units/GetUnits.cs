using Microsoft.AspNetCore.Http.HttpResults;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Features.Units;

public static class GetUnits
{
    public sealed record Response();

    public sealed class AuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<Response>
    {
        public Task<bool> IsAuthorized(Response request)
        {
            return Task.FromResult(currentUser.HasRole(UserRoles.Admin));
        }
    }

    [GroupEndpoint("Units")]
    public class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardAuthenticatedGet<Response>(string.Empty, Handler)
                     .WithName("GetMeasurementUnits")
                     .WithDescription("Gets all measurement units");
        }
    }

    internal static async Task<Results<Ok<Response>, NotFound>> Handler()
    {
        // Logic for changing the password goes here
        return TypedResults.Ok(new Response());
    }
}

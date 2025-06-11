using Microsoft.AspNetCore.Http.HttpResults;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Identity.API.Features.Authorization;

public static class RefreshToken
{
    [GroupEndpoint("Auth")]
    public sealed class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/refresh-token", Handler)
                     .WithName("RefreshToken")
                     .WithDescription("Refreshes user auth token");
        }
    }

    internal static async Task<Results<Ok<string>, NotFound>> Handler()
    {
        // Logic for changing the password goes here
        return TypedResults.Ok("Refreshed token");
    }
}

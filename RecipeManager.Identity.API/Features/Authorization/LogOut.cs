using Microsoft.AspNetCore.Http.HttpResults;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Identity.API.Features.Authorization;

public static class LogOut
{
    [GroupEndpoint("Auth")]
    public sealed class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/logout", Handler)
                     .WithName("Logout")
                     .WithDescription("Signs user out");
        }
    }

    internal static async Task<Results<Ok<string>, NotFound>> Handler()
    {
        // Logic for changing the password goes here
        return TypedResults.Ok("Logged out");
    }
}

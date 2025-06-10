using Microsoft.AspNetCore.Http.HttpResults;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Identity.API.Features.Authorization;

public static class LogIn
{
    [GroupEndpoint("Auth")]
    public sealed class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/login", Handler)
                     .WithName("Login")
                     .WithDescription("Signs user in");
        }
    }

    internal static async Task<Results<Ok<string>, NotFound>> Handler()
    {
        // Logic for changing the password goes here
        return TypedResults.Ok("Logged in");
    }
}

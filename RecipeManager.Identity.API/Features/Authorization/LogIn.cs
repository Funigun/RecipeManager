using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Identity.API.Features.Authorization;

public static class LogIn
{
    [GroupEndpoint("Auth")]
    public sealed class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("Login", () =>
            {
                // Logic for changing the password goes here
                return Results.Ok("Logged in.");
            });
        }
    }
}

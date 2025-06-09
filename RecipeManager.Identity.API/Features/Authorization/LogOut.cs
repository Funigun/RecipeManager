using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Identity.API.Features.Authorization;

public static class LogOut
{
    [GroupEndpoint("Auth")]
    public sealed class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("Logout", () =>
            {
                // Logic for changing the password goes here
                return Results.Ok("Logged out.");
            });
        }
    }
}

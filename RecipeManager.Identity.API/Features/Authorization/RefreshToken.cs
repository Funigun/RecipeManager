using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Identity.API.Features.Authorization;

public static class RefreshToken
{
    [GroupEndpoint("Auth")]
    public sealed class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("{userId}/RefreshToken", () =>
            {
                // Logic for changing the password goes here
                return Results.Ok("Token refreshed.");
            });
        }
    }
}

using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Identity.API.Features.UsersManagement;

public static class GetUsersQuery
{
    [GroupEndpoint("Admin")]
    public sealed class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("Users", () =>
            {
                // Logic for changing the password goes here
                return Results.Ok("Received users");
            });
        }
    }
}

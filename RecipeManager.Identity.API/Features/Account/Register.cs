using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Identity.API.Features.Account;

public static class Register
{
    [GroupEndpoint("Account")]
    public sealed class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("Register", () =>
            {
                // Logic for changing the password goes here
                return Results.Ok("Acount created.");
            });
        }
    }
}

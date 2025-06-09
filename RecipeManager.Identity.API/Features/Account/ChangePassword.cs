using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Identity.API.Features.Account;

public static class ChangePassword
{
    [GroupEndpoint("Account")]
    public sealed class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("{userId}/ChangePassword", () =>
            {
                // Logic for changing the password goes here
                return Results.Ok("Password changed successfully.");
            });
        }
    }
}

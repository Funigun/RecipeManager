using Microsoft.AspNetCore.Http.HttpResults;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Identity.API.Features.Account;

public static class ChangePassword
{
    [GroupEndpoint("Account")]
    public class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPut("/change-password/{userId}", Handler)
                     .WithName("ChangePassword")
                     .WithDescription("Changes account password");
        }
    }

    internal static async Task<Results<Ok<string>, NotFound>> Handler()
    {
        // Logic for changing the password goes here
        return TypedResults.Ok("Password changed successfully.");
    }
}

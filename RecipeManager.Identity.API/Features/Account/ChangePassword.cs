using Microsoft.AspNetCore.Http.HttpResults;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Identity.Api.Features.Account;

public static class ChangePassword
{
    public class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPut(AccountGroup.ChangePasswordRoute, Handler)
                     .WithName("ChangePassword")
                     .WithDescription("Changes account password")
                     .WithTags(AccountGroup.Tag);
        }
    }

    internal static async Task<Results<Ok<string>, NotFound>> Handler()
    {
        // Logic for changing the password goes here
        return TypedResults.Ok("Password changed successfully.");
    }
}

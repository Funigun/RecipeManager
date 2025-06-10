using Microsoft.AspNetCore.Http.HttpResults;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Identity.API.Features.UsersManagement;

public static class GetRolesQuery
{
    [GroupEndpoint("Admin")]
    public sealed class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/roles", Handler)
                     .WithName("Roles")
                     .WithDescription("Get available roles and permissions");
        }
    }

    internal static async Task<Results<Ok<string>, NotFound>> Handler()
    {
        // Logic for changing the password goes here
        return TypedResults.Ok("Received roles and permissions");
    }
}

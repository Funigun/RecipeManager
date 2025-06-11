using Microsoft.AspNetCore.Http.HttpResults;
using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Identity.API.Features.Account;

public static class Register
{
    public record Request(string UserName);
    public record Response(string Token);

    [GroupEndpoint("Account")]
    public class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/register", Handler)
                     .WithName("Register")
                     .WithDescription("Creates new user account");
        }
    }

    internal static async Task<Results<Ok<Response>, NotFound>> Handler(Request request)
    {
        // Logic for changing the password goes here
        return TypedResults.Ok(new Response("asd"));
    }
}

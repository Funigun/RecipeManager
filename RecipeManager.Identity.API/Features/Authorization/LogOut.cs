using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Shared.Contracts.Authorization;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Identity.Api.Persistance;

namespace RecipeManager.Identity.Api.Features.Authorization;

public static class LogOut
{
    public sealed class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost(AuthGroup.LogOutRoute, Handler)
                     .WithName("Logout")
                     .WithDescription("Signs user out")
                     .WithTags(AuthGroup.Tag)
                     .RequireAuthorization();
        }
    }

    internal static async Task<Results<Ok, NotFound>> Handler(ICurrentUser currentUser, AppDbContext dbContext, CancellationToken cancellationToken)
    {
        await dbContext.RefreshTokens.Where(token => token.User.Id == int.Parse(currentUser.Id))
                                     .ExecuteDeleteAsync(cancellationToken);

        return TypedResults.Ok();
    }
}

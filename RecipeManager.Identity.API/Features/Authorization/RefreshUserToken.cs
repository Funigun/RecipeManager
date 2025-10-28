using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Identity.Api.Domain;
using RecipeManager.Identity.Api.Features.Common;
using RecipeManager.Identity.Api.Persistance;

namespace RecipeManager.Identity.Api.Features.Authorization;

public static class RefreshUserToken
{
    public sealed record Request(string RefreshToken);

    public sealed record Response(string Token, string RefreshToken, DateTime ExpirationDate);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator(AppDbContext dbContext)
        {
            RuleFor(request => request.RefreshToken)
                .MustAsync(async (refreshToken, cancellationToken) =>
                {
                    RefreshToken? token = await dbContext.RefreshTokens.AsNoTracking()
                                                                       .FirstOrDefaultAsync(token => token.Token == refreshToken, cancellationToken);
                    return token != null && token.ExpirationDate > DateTime.UtcNow;
                }).WithMessage("Invalid or expired refresh token");
        }
    }

    [GroupEndpoint("Auth")]
    public sealed class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/refresh-token", Handler)
                     .WithName("RefreshToken")
                     .WithDescription("Refreshes user auth token")
                     .RequireAuthorization();
        }
    }

    internal static async Task<Results<Ok<Response>, NotFound>> Handler([FromQuery] string refreshToken, UserManager<User> userManager, AppDbContext dbContext, AuthorizationService authorizationService, IConfiguration configuration, CancellationToken cancellationToken)
    {
        RefreshToken? existingToken = await dbContext.RefreshTokens.Include(token => token.User)
                                                                   .FirstAsync(token => token.Token == refreshToken, cancellationToken);

        DateTime tokenExpirationTime = authorizationService.CalculateTokenExpirationTime(configuration, isRefreshToken: false);
        DateTime refreshTokenExpirationTime = authorizationService.CalculateTokenExpirationTime(configuration, isRefreshToken: true);

        string token = await authorizationService.GenerateToken(existingToken.User, userManager, configuration, isRefreshToken: false, tokenExpirationTime);

        existingToken.Token = authorizationService.GenerateRefreshToken();
        existingToken.ExpirationDate = refreshTokenExpirationTime;

        dbContext.RefreshTokens.Update(existingToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(new Response(token, existingToken.Token, tokenExpirationTime));
    }
}

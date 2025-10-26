using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Identity.Api.Domain;
using RecipeManager.Identity.Api.Features.Common;
using RecipeManager.Identity.Api.Persistance;

namespace RecipeManager.Identity.Api.Features.Authorization;

public static class LogIn
{
    public sealed record Request(string UserName, string Password);

    public sealed record Response(string Token, string RefreshToken, DateTime ExpirationDate);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator(UserManager<User> userManager)
        {
            RuleFor(request => request)
                .MustAsync(async (request, cancellationToken) =>
                {
                    User? user = await userManager.Users.FirstOrDefaultAsync(userManager => userManager.UserName == request.UserName, cancellationToken);

                    return user != null && await userManager.CheckPasswordAsync(user, request.Password);
                }).WithMessage("User Name or password is not correct");
        }
    }

    [GroupEndpoint("Auth")]
    public sealed class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardValidatedPost<Request, Response>("/login", Handler)
                     .WithName("Login")
                     .WithDescription("Signs user in");
        }
    }

    internal static async Task<Results<Ok<Response>, BadRequest>> Handler(Request request, UserManager<User> userManager, AuthorizationService authorizationService, AppDbContext dbContext, IConfiguration configuration, CancellationToken cancellationToken)
    {
        User user = await userManager.Users.FirstAsync(userManager => userManager.UserName == request.UserName, cancellationToken);

        DateTime tokenExpirationTime = authorizationService.CalculateTokenExpirationTime(configuration, isRefreshToken: false);
        DateTime refreshTokenExpirationTime = authorizationService.CalculateTokenExpirationTime(configuration, isRefreshToken: true);

        string token = await authorizationService.GenerateToken(user, userManager, configuration, isRefreshToken: false, tokenExpirationTime);

        RefreshToken refreshToken = new()
        {
            Token = authorizationService.GenerateRefreshToken(),
            ExpirationDate = refreshTokenExpirationTime,
            User = user
        };

        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        Response response = new(token, refreshToken.Token, tokenExpirationTime);

        return TypedResults.Ok(response);
    }
}

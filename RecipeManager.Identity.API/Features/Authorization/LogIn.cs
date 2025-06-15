using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Identity.API.Domain;
using RecipeManager.Shared.Contracts.User.Login;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RecipeManager.Identity.API.Features.Authorization;

public static class LogIn
{
    public sealed record Request(string UserName, string Password);

    public sealed record Response(UserLoginResponseDto UserLoginResponseDto) { };

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

    internal static async Task<Results<Ok<Response>, BadRequest>> Handler(Request request, UserManager<User> userManager, IConfiguration configuration, CancellationToken cancellationToken)
    {
        User user = await userManager.Users.FirstAsync(userManager => userManager.UserName == request.UserName, cancellationToken);

        UserLoginResponseDto response = new()
        {
            Token = await GenerateToken(user, userManager, configuration),
        };

        // Logic for changing the password goes here
        return TypedResults.Ok(new Response(response));
    }

    private static async Task<string> GenerateToken(User user, UserManager<User> userManager, IConfiguration configuration)
    {
        SymmetricSecurityKey? securitykey = new(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]!));
        SigningCredentials? credentials = new(securitykey, SecurityAlgorithms.HmacSha256);

        IList<string>? roles = await userManager.GetRolesAsync(user);
        IList<Claim>? roleClaims = roles.Select(q => new Claim(ClaimTypes.Role, q)).ToList();

        IList<Claim>? userClaims = await userManager.GetClaimsAsync(user);

        List<Claim> claims = 
        [
            new ("Id", user.Id.ToString()),
            new (JwtRegisteredClaimNames.Sub, user.UserName!),
        ];

        claims.AddRange(userClaims);
        claims.AddRange(roleClaims);

        JwtSecurityToken? token = new
        (
            issuer: configuration["JwtSettings:Issuer"],
            audience: configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(configuration["JwtSettings:Duration"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

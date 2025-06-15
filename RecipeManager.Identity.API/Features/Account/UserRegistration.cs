using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Api.Shared.Endpoint;
using RecipeManager.Identity.API.Common.Exceptions;
using RecipeManager.Identity.API.Domain;
using RecipeManager.Shared.Contracts.Authorization;
using RecipeManager.Shared.Contracts.User.Registration;

namespace RecipeManager.Identity.API.Features.Account;

public static class UserRegistration
{
    public record Request(string UserName, string Password, string Email);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator(UserManager<User> userManager)
        {
            RuleFor(request => request.UserName)
                .SetValidator(new UserNameValidator())
                .MustAsync
                (
                    async (name, cancellationToken) =>
                    {
                        bool userExists = await userManager.FindByNameAsync(name) != null;
                        return !userExists;
                    }
                ).WithMessage("User Name is already in use");

            RuleFor(request => request.Password)
                .SetValidator(new PasswordValidator());

            RuleFor(request => request.Email)
                .SetValidator(new EmailValidator());
        }
    }

    [GroupEndpoint("Account")]
    public class Enpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapStandardValidatedPost<Request, string>("/register", Handler)
                     .WithName("Register")
                     .WithDescription("Creates new user account");
        }
    }

    internal static async Task<Results<Ok, BadRequest>> Handler(Request request, UserManager<User> userManager, CancellationToken cancellationToken)
    {
        User user = request.ToUser();

        if (await userManager.Users.AnyAsync(usr => usr.UserName!.Equals(user.UserName, StringComparison.CurrentCultureIgnoreCase), cancellationToken))
        {
            throw IdentityValidationException.UserNameAlreadyInUse();
        }

        IdentityResult result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            throw IdentityValidationException.RegistrationFailed(result.Errors.Select(error => error.Description));
        }

        await userManager.AddToRoleAsync(user, UserRoles.User);

        return TypedResults.Ok();
    }

    internal static User ToUser(this Request userDto)
    {
        return new User
        {
            UserName = userDto.UserName,
            Email = userDto.Email,
        };
    }
}

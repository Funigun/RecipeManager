using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Identity.API.Features.Account;

public class AccountGroup : IGroupEndpoint
{
    public string GroupName => "Account";

    public void Configure(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.WithDescription("Account management features")
                    .WithTags("account");
    }
}

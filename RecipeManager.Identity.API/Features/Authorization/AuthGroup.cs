using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Identity.API.Features.Authorization;

public class AuthGroup : IGroupEndpoint
{
    public string GroupName => "Auth";

    public void Configure(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.WithDescription("Authorization group")
                    .WithTags("auth");
    }
}

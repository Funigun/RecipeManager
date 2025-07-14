using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Identity.Api.Features.Admin;

public sealed class AdminGroup : IGroupEndpoint
{
    public string GroupName { get; } = "Admin";

    public void Configure(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.WithDescription("Admin features")
                    .WithTags("admin");
    }
}

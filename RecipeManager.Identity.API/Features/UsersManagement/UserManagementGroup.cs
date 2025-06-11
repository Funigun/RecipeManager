using RecipeManager.Api.Shared.Endpoint;

namespace RecipeManager.Identity.API.Features.UsersManagement;

public class UserManagementGroup : IGroupEndpoint
{
    public string GroupName => "Admin";

    public void Configure(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.WithDescription("Admin panel")
                    .WithTags("admin");
    }
}
using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace RecipeManager.Api.Shared.Contracts.Authorization;

internal sealed class CurrentUser : ICurrentUser
{
    public int Id { get; set; }

    public IEnumerable<string> Roles { get; set; } = [];

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        Id = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value);

        Roles = httpContextAccessor.HttpContext?.User?.Claims.Where(claim => claim.Type == ClaimTypes.Role)
                                                             .Select(claim => claim.Value) ?? [];
    }

    public bool HasRole(string role)
    {
        return Roles.Contains(role);
    }
}

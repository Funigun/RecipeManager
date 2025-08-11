using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace RecipeManager.Api.Shared.Contracts.Authorization;

internal sealed class CurrentUser : ICurrentUser
{
    public string Id { get; set; }

    public IEnumerable<string> Roles { get; set; } = [];

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        Id = httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value ?? string.Empty;

        Roles = httpContextAccessor.HttpContext?.User?.Claims.Where(claim => claim.Type == ClaimTypes.Role)
                                                             .Select(claim => claim.Value) ?? [];
    }

    public bool HasRole(string role)
    {
        return Roles.Contains(role);
    }
}

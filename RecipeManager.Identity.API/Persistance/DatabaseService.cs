using Microsoft.EntityFrameworkCore;
using RecipeManager.Identity.API.Domain;
using RecipeManager.Shared.Contracts.Authorization;

namespace RecipeManager.Identity.API.Persistance;

public class DatabaseService(AppDbContext dbContext)
{
    public async Task InitDatabase()
    {
        await SeedRoles();
    }

    private async Task SeedRoles()
    {
        IEnumerable<Role> existingRoles = await dbContext.Roles.ToListAsync();
        IEnumerable<string> missingRoles = UserRoles.AllRoles
                                                   .Where(roleName => !existingRoles.Any(role => role.Name == roleName));

        if (!missingRoles.Any())
        {
            IEnumerable<Role> rolesToAdd = missingRoles.Select(roleName => new Role { Name = roleName, NormalizedName = roleName.ToUpper() });

            await dbContext.Roles.AddRangeAsync(rolesToAdd);
            await dbContext.SaveChangesAsync();
        }
    }
}

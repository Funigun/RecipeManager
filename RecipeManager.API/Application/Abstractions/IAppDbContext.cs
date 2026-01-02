using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RecipeManager.Api.Domain.Cookbooks;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.MealPlan;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Units;

namespace RecipeManager.Api.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<Unit> Units { get; set; }

    DbSet<RecipeCategory> RecipeCategories { get; set; }

    DbSet<Recipe> Recipes { get; set; }

    DbSet<IngredientCategory> IngredientCategories { get; set; }

    DbSet<Ingredient> Ingredients { get; set; }

    DbSet<CookbookCategory> CookbookCategories { get; set; }

    DbSet<Cookbook> Cookbooks { get; set; }

    DbSet<MealPlan> MealPlans { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    Task<IDbContextTransaction> BeginTransaction(CancellationToken cancellationToken);

    Task CommitTransaction(CancellationToken cancellationToken);

    Task RollbackTransaction(CancellationToken cancellationToken);
}

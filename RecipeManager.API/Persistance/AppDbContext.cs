using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RecipeManager.Api.Application.Abstractions;
using RecipeManager.Api.Domain.Common.Abstractions;
using RecipeManager.Api.Domain.Cookbooks;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.MealPlan;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Units;
using RecipeManager.Api.Persistance.Configuration.Cookbooks.Converters;
using RecipeManager.Api.Persistance.Configuration.Id;
using RecipeManager.Api.Persistance.Configuration.Ingredients.Converters;
using RecipeManager.Api.Persistance.Configuration.MealPlanner.Converters;
using RecipeManager.Api.Persistance.Configuration.Recipes.Converters;
using RecipeManager.Api.Persistance.Configuration.Units;
using RecipeManager.Api.Shared.Contracts.Authorization;

namespace RecipeManager.Api.Persistance;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    private readonly ICurrentUser _currentUser = default!;

    public DbSet<Unit> Units { get; set; }

    public DbSet<RecipeCategory> RecipeCategories { get; set; }

    public DbSet<Recipe> Recipes { get; set; }

    public DbSet<IngredientCategory> IngredientCategories { get; set; }

    public DbSet<Ingredient> Ingredients { get; set; }

    public DbSet<CookbookCategory> CookbookCategories { get; set; }

    public DbSet<Cookbook> Cookbooks { get; set; }

    public DbSet<MealPlan> MealPlans { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUser currentUser) : this(options)
    {
        _currentUser = currentUser;
    }

    public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateEntities();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task<IDbContextTransaction> BeginTransaction(CancellationToken cancellationToken)
    {
        return await Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransaction(CancellationToken cancellationToken)
    {
        await Database.CommitTransactionAsync(cancellationToken);
    }

    public async Task RollbackTransaction(CancellationToken cancellationToken)
    {
        await Database.RollbackTransactionAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Conventions.Add(_ => new GuidFinalizingConvention());

        configurationBuilder.Properties<UnitId>().HaveConversion<UnitIdConverter>();

        configurationBuilder.Properties<IngredientCategoryId>().HaveConversion<IngredientCategoryIdConverter>();
        configurationBuilder.Properties<IngredientId>().HaveConversion<IngredientIdConverter>();

        configurationBuilder.Properties<RecipeCategoryId>().HaveConversion<RecipeCategoryIdConverter>();
        configurationBuilder.Properties<RecipeId>().HaveConversion<RecipeIdConverter>();

        configurationBuilder.Properties<CookbookCategoryId>().HaveConversion<CookbookCategoryIdConverter>();
        configurationBuilder.Properties<CookbookId>().HaveConversion<CookbookIdConverter>();

        configurationBuilder.Properties<MealPlanId>().HaveConversion<MealPlanIdConverter>();
    }

    private void UpdateEntities()
    {
        DateTime currentDateTime = DateTime.UtcNow;

        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<AuditableEntity> entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = _currentUser.Id.ToString();
                    entry.Entity.CreatedOn = currentDateTime;
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedBy = _currentUser.Id.ToString();
                    entry.Entity.ModifiedOn = currentDateTime;
                    break;
            }
        }
    }
}

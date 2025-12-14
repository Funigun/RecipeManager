using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeManager.Api.Domain.MealPlan;

namespace RecipeManager.Api.Persistance.Configuration.MealPlanner;

public class MealPlanConfiguration : IEntityTypeConfiguration<MealPlan>
{
    public void Configure(EntityTypeBuilder<MealPlan> builder)
    {
        builder.ToTable("MealPlans");

        builder.Property(plan => plan.Id)
               .ValueGeneratedOnAdd();

        builder.OwnsMany(plan => plan.Recipes, recipes =>
        {
            recipes.ToTable("RecipeToMealPlan");
        });
    }
}

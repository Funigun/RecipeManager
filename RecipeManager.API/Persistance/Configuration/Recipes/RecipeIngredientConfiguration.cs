using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Units;

namespace RecipeManager.Api.Persistance.Configuration.Recipes;

public sealed class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
    {
        builder.ToTable("RecipeIngredient");

        builder.HasOne<Recipe>()
               .WithMany(r => r.Ingredients)
               .HasForeignKey(rc => rc.RecipeId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Ingredient>()
               .WithMany()
               .HasForeignKey(rc => rc.IngredientId)
               .IsRequired()
               .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasOne<Unit>()
               .WithMany()
               .HasForeignKey(rc => rc.UnitId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}

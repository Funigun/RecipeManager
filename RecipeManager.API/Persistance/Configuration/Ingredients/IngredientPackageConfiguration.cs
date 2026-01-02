using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Units;

namespace RecipeManager.Api.Persistance.Configuration.Ingredients;

public sealed class IngredientPackageConfiguration : IEntityTypeConfiguration<IngredientPackage>
{
    public void Configure(EntityTypeBuilder<IngredientPackage> builder)
    {
        builder.ToTable("IngredientPackage");

        builder.HasOne<Ingredient>()
               .WithOne(i => i.IngredientPackage)
               .HasForeignKey<IngredientPackage>(ip => ip.IngredientId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Unit>()
               .WithMany()
               .HasForeignKey(ip => ip.PackageUnitId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Unit>()
               .WithMany()
               .HasForeignKey(ip => ip.PackageSizeUnitId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}

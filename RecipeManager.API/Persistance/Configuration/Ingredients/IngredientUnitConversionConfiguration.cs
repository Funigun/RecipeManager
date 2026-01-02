using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Units;

namespace RecipeManager.Api.Persistance.Configuration.Ingredients;

public sealed class IngredientUnitConversionConfiguration : IEntityTypeConfiguration<IngredientUnitConvertion>
{
    public void Configure(EntityTypeBuilder<IngredientUnitConvertion> builder)
    {
        builder.ToTable("IngredientUnitConversion");

        builder.HasOne<Ingredient>()
               .WithMany(i => i.IngredientUnitConvertions)
               .HasForeignKey(uc => uc.IngredientId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Unit>()
               .WithMany()
               .HasForeignKey(uc => uc.UnitToConvertId)
               .IsRequired()
               .OnDelete(DeleteBehavior.ClientCascade);
    }
}

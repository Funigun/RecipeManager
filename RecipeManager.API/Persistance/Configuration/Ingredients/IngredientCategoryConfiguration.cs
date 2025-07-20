using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeManager.Api.Domain.Ingredients;

namespace RecipeManager.Api.Persistance.Configuration.Ingredients;

public sealed class IngredientCategoryConfiguration : IEntityTypeConfiguration<IngredientCategory>
{
    public void Configure(EntityTypeBuilder<IngredientCategory> builder)
    {
        builder.ToTable("IngredientCategory");

        builder.Property(category => category.Id)
               .ValueGeneratedOnAdd();

        builder.Property(builder => builder.Name)
               .HasMaxLength(IngredientCategoryDomainValidator.CategoryNameMaxLength)
               .IsRequired(true);

        builder.HasMany(category => category.Subcategories)
               .WithOne()
               .HasForeignKey(category => category.ParentId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeManager.Api.Domain.Cookbooks;

namespace RecipeManager.Api.Persistance.Configuration.Cookbooks;

public sealed class CookbookCategoryConfiguration : IEntityTypeConfiguration<CookbookCategory>
{
    public void Configure(EntityTypeBuilder<CookbookCategory> builder)
    {
        builder.ToTable("CookbookCategory");

        builder.Property(category => category.Id)
               .ValueGeneratedOnAdd();

        builder.Property(category => category.Name)
               .HasMaxLength(CookbookCategoryDomainValidator.CookbookCategoryNameMaxLength)
               .IsRequired(true);

        builder.HasMany(category => category.Subcategories)
               .WithOne()
               .HasForeignKey(category => category.ParentId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.OwnsMany(category => category.Recipes, configuration =>
        {
            configuration.ToTable("CookbookCategoryToRecipe");
        });
    }
}

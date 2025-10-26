using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Recipes.Validators;

namespace RecipeManager.Api.Persistance.Configuration.Recipes;

public sealed class RecipeCategoryConfiguration : IEntityTypeConfiguration<RecipeCategory>
{
    public void Configure(EntityTypeBuilder<RecipeCategory> builder)
    {
        builder.ToTable("RecipeCategory");

        builder.Property(category => category.Id)
               .ValueGeneratedOnAdd();

        builder.Property(category => category.Name)
               .HasMaxLength(RecipeCategoryDomainValidator.RecipeCategoryNameMaxLength)
               .IsRequired(true);

        builder.Property(category => category.Type)
               .IsRequired(true);
    }
}

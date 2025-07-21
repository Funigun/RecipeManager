using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeManager.Api.Domain.Ingredients;

namespace RecipeManager.Api.Persistance.Configuration.Ingredients;

public sealed class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.ToTable("Ingredient");

        builder.Property(ingredient => ingredient.Id)
               .ValueGeneratedOnAdd();

        builder.Property(ingredient => ingredient.Name)
               .HasMaxLength(IngredientDomainValidator.IngredientNameMaxLength)
               .IsRequired(true);

        builder.OwnsMany(ingredient => ingredient.Categories, categories =>
        {
            categories.ToTable("IngredientToIngredientCategory");
        });

        builder.OwnsMany(ingredient => ingredient.Recipes, recipes =>
        {
            recipes.ToTable("IngredientToRecipe");
        });
    }
}

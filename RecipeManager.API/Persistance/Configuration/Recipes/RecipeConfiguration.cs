using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeManager.Api.Domain.Ingredients;
using RecipeManager.Api.Domain.Recipes;
using RecipeManager.Api.Domain.Recipes.Validators;

namespace RecipeManager.Api.Persistance.Configuration.Recipes;

public sealed class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.ToTable("Recipe");

        builder.Property(recipe => recipe.Id)
               .ValueGeneratedOnAdd();

        builder.Property(recipe => recipe.Title)
               .HasMaxLength(RecipeDomainValidator.RecipeTitleMaxLength)
               .IsRequired(true);

        builder.Property(recipe => recipe.Description)
               .HasMaxLength(RecipeDomainValidator.RecipeDescriptionMaxLength)
               .IsRequired(false);

        builder.Property(recipe => recipe.NumberOfServings)
               .HasDefaultValue(RecipeDomainValidator.MinimumNumberOfServings)
               .IsRequired(true);

        builder.Property(recipe => recipe.IngredientId)
               .IsRequired(false);

        builder.HasOne<Ingredient>()
               .WithMany()
               .HasForeignKey(recipe => recipe.IngredientId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.OwnsOne(recipe => recipe.Amount, recipeAmount =>
        {
            recipeAmount.Property(amount => amount.Amount)
                        .HasColumnName("Amount");

            recipeAmount.HasOne(amount => amount.Unit)
                        .WithMany()
                        .HasForeignKey(amount => amount.UnitId)
                        .IsRequired(false);
        });

        builder.OwnsMany(recipe => recipe.Sections, recipeSection =>
        {
            recipeSection.ToTable("RecipeSection");

            recipeSection.WithOwner()
                         .HasForeignKey("RecipeId");

            recipeSection.Property<int>("Id");
            recipeSection.HasKey("Id");

            recipeSection.Property(section => section.Type)
                         .IsRequired();

            recipeSection.OwnsMany(section => section.Steps, recipeStep =>
            {
                recipeStep.ToTable("RecipeSectionStep");

                recipeStep.Property(step => step.Order)
                          .HasColumnName("StepOrder")
                          .IsRequired();

                recipeStep.Property(step => step.Description)
                          .HasMaxLength(RecipeDomainValidator.RecipeStepDescriptionMaxLength)
                          .HasColumnName("StepDescription")
                          .IsRequired();

                recipeStep.Property(step => step.ImageUrl)
                          .HasColumnName("StepImage")
                          .IsRequired(false);
            });
        });
    }
}

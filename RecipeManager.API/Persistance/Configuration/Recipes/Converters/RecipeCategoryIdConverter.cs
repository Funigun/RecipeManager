using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RecipeManager.Api.Domain.Recipes;

namespace RecipeManager.Api.Persistance.Configuration.Recipes.Converters;

public sealed class RecipeCategoryIdConverter : ValueConverter<RecipeCategoryId, Guid>
{
    public RecipeCategoryIdConverter()
         : base(id => id.Value, value => new RecipeCategoryId(value))
    {
    }
}

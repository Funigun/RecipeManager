using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RecipeManager.Api.Domain.Recipes;

namespace RecipeManager.Api.Persistance.Configuration.Recipes.Converters;

public sealed class RecipeIdConverter : ValueConverter<RecipeId, Guid>
{
    public RecipeIdConverter()
        : base(id => id.Value, value => new RecipeId(value))
    {
    }
}

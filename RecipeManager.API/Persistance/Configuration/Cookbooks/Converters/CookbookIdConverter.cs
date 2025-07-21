using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RecipeManager.Api.Domain.Cookbooks;

namespace RecipeManager.Api.Persistance.Configuration.Cookbooks.Converters;

public sealed class CookbookIdConverter : ValueConverter<CookbookId, Guid>
{
    public CookbookIdConverter()
         : base(id => id.Value, value => new CookbookId(value))
    {
    }
}

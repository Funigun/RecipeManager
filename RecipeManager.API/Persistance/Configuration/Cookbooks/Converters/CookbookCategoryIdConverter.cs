using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RecipeManager.Api.Domain.Cookbooks;

namespace RecipeManager.Api.Persistance.Configuration.Cookbooks.Converters;

public sealed class CookbookCategoryIdConverter : ValueConverter<CookbookCategoryId, Guid>
{
    public CookbookCategoryIdConverter()
         : base(id => id.Value, value => new CookbookCategoryId(value))
    {
    }
}

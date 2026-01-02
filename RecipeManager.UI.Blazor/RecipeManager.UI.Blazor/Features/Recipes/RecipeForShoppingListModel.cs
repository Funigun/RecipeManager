using System.Text.Json.Serialization;

namespace RecipeManager.UI.Blazor.Features.Recipes;

public sealed class RecipeForShoppingListModel
{
    public Guid Id { get; set; }

    [JsonIgnore]
    public string Title { get; set; } = string.Empty;

    [JsonIgnore]
    public string? ImageUrl { get; set; }

    public int NumberOfServings { get; set; }
}

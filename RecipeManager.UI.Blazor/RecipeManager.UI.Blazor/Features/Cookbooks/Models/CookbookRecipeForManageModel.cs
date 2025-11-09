using System.Text.Json.Serialization;

namespace RecipeManager.UI.Blazor.Features.Cookbooks.Models;

public class CookbookRecipeForManageModel
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    [JsonIgnore]
    public string DropZoneIndetifier { get; set; } = "Zone";
}

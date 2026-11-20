using MudBlazor;

namespace RecipeManager.UI.Shared.Appearance;

public sealed record ThemeDefinition(string Id, string Name, MudTheme Theme, bool IsDarkMode);

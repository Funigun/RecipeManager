namespace RecipeManager.UI.Shared.Appearance;

public sealed class ThemeService
{
    private ThemeDefinition _currentTheme = ThemeCatalog.Themes[0];

    public ThemeDefinition Current => _currentTheme;

    public void SelectTheme(string themeId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(themeId);

        ThemeDefinition? theme = ThemeCatalog.Themes.FirstOrDefault(
            availableTheme => string.Equals(availableTheme.Id, themeId, StringComparison.Ordinal));

        if (theme is null)
        {
            throw new ArgumentOutOfRangeException(nameof(themeId), themeId, "The requested theme is not registered.");
        }

        if (string.Equals(_currentTheme.Id, theme.Id, StringComparison.Ordinal))
        {
            return;
        }

        _currentTheme = theme;
    }
}

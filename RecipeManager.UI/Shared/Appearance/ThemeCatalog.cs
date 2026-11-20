using MudBlazor;

namespace RecipeManager.UI.Shared.Appearance;

public static class ThemeCatalog
{
    private static readonly MudTheme FreshKitchenTheme = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#2D6A4F",
            Secondary = "#E76F51",
            Tertiary = "#F4A261",
            Success = "#52B788",
            Warning = "#E9C46A",
            Background = "#FAF9F6",
            BackgroundGray = "#F4F7F5",
            Surface = "#FFFFFF",
            AppbarBackground = "#FAF9F6",
            AppbarText = "#1A202C",
            DrawerBackground = "#FFFFFF",
            DrawerText = "#1A202C",
            TextPrimary = "#1A202C",
            TextSecondary = "#64748B",
            Divider = "#E2E8F0",
            LinesDefault = "#E2E8F0",
            LinesInputs = "#E2E8F0",
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#52B788",
            Secondary = "#F4A261",
            Tertiary = "#E9C46A",
            Success = "#52B788",
            Warning = "#F4A261",
            Background = "#0F1715",
            BackgroundGray = "#1A2C26",
            Surface = "#162420",
            AppbarBackground = "#0F1715",
            AppbarText = "#F3F5F4",
            DrawerBackground = "#162420",
            DrawerText = "#F3F5F4",
            TextPrimary = "#F3F5F4",
            TextSecondary = "#A1ADA6",
            Divider = "#294039",
            LinesDefault = "#294039",
            LinesInputs = "#294039",
        },
    };

    public static IReadOnlyList<ThemeDefinition> Themes { get; } =
    [
        new ThemeDefinition("light", "Light", FreshKitchenTheme, false),
        new ThemeDefinition("dark", "Dark", FreshKitchenTheme, true),
    ];
}

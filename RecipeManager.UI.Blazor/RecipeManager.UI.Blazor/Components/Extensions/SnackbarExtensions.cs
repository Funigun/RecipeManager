using MudBlazor;

namespace RecipeManager.UI.Blazor.Components.Extensions;

public static class SnackbarExtensions
{
    public static void ShowError(this ISnackbar snackbar, string message)
    {
        snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;        
        snackbar.Add(message, Severity.Error);
    }

    public static void ShowSuccess(this ISnackbar snackbar, string message)
    {
        snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
        snackbar.Add(message, Severity.Success);
    }

    public static void ShowInfo(this ISnackbar snackbar, string message)
    {
        snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
        snackbar.Add(message, Severity.Info);
    }

    public static void ShowWarning(this ISnackbar snackbar, string message)
    {
        snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
        snackbar.Add(message, Severity.Warning);
    }
}

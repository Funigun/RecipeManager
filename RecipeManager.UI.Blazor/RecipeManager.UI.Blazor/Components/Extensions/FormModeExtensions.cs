using RecipeManager.UI.Blazor.Components.Common;

namespace RecipeManager.UI.Blazor.Components.Extensions;

public static class FormModeExtensions
{
    public static bool IsCreateMode(this FormMode mode) => mode == FormMode.Create;

    public static bool IsEditMode(this FormMode mode) => mode == FormMode.Edit;

    public static bool IsViewMode(this FormMode mode) => mode == FormMode.View;

    public static bool IsCreateOrEditMode(this FormMode mode) => mode is FormMode.Create or FormMode.Edit;
}

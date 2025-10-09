namespace RecipeManager.UI.Blazor.Components.Common.DialogMessage;

public interface IDialogMessageService
{
    Task<bool> ShowDeleteConfirmationMessage();

    Task<bool> ShowDeleteItemsConfirmationMessage(int numberOfItems);
}

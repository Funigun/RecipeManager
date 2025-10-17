using MudBlazor;

namespace RecipeManager.UI.Blazor.Components.Common.DialogMessage;

public sealed class DialogMessageService(IDialogService dialogService) : IDialogMessageService
{
    public async Task<bool> ShowDeleteConfirmationMessage()
    {
        DialogParameters<DialogMessageComponent> parameters = new()
        {
            { x => x.ContentText, "Do you really want to delete these item? This process cannot be undone." },
            { x => x.ButtonText, "Delete" },
            { x => x.Color, Color.Error }
        };

        DialogOptions options = new() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

        IDialogReference dialog = await dialogService.ShowAsync<DialogMessageComponent>("Delete", parameters, options);

        DialogResult result = (await dialog.Result)!;

        return result is not null && !result.Canceled;
    }

    public async Task<bool> ShowDeleteItemsConfirmationMessage(int numberOfItems)
    {
        DialogParameters<DialogMessageComponent> parameters = new()
        {
            { x => x.ContentText, $"Do you really want to delete selected items ({numberOfItems})?" },
            { x => x.ButtonText, "Delete" },
            { x => x.Color, Color.Error }
        };

        DialogOptions options = new() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

        IDialogReference dialog = await dialogService.ShowAsync<DialogMessageComponent>("Delete", parameters, options);

        DialogResult result = (await dialog.Result)!;

        return result is not null && !result.Canceled;
    }

    public async Task<bool> CancelOperationConfirmed()
    {
        DialogParameters<DialogMessageComponent> parameters = new()
        {
            { x => x.ContentText, "Are you sure you want to cancel operation? All changes will be lost." },
            { x => x.ButtonText, "Ok" },
            { x => x.Color, Color.Warning }
        };

        DialogOptions options = new() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

        IDialogReference dialog = await dialogService.ShowAsync<DialogMessageComponent>("Cancelling operation confirmation", parameters, options);

        DialogResult result = (await dialog.Result)!;

        return result is not null && !result.Canceled;
    }
}

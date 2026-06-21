using eSecurity.Client.Components.Access;
using eSecurity.Core.Security.Authorization.Verification;
using MudBlazor;

namespace eSecurity.Client.Common.Confirmation;

public class ConfirmationManager(IDialogService dialogService)
{
    private readonly IDialogService _dialogService = dialogService;

    public async ValueTask<DialogResult> InitiateAsync(OperationType operationType)
    {
        var context = new ConfirmationContext
        {
            Operation = operationType
        };

        var parameters = new DialogParameters<AccessConfirmationDialog>
        {
            { dialog => dialog.Context, context }
        };

        var options = new DialogOptions
        {
            BackdropClick = true,
            CloseOnEscapeKey = true,
            CloseButton = true,
            MaxWidth = MaxWidth.ExtraExtraLarge
        };

        const string title = "Access confirmation";
        var dialog = await _dialogService.ShowAsync<AccessConfirmationDialog>(title, parameters, options);
        var result = await dialog.Result;
        return result!;
    }
}
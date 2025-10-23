using eAccount.Blazor.Server.UI.Components.Common.Access;
using eSystem.Domain.Security.Verification;

namespace eAccount.Blazor.Server.UI.Utilities;

public class ConfirmationManager(IDialogService dialogService)
{
    private readonly IDialogService dialogService = dialogService;

    public async ValueTask<DialogResult> InitiateAsync(PurposeType purpose, ActionType action)
    {
        var context = new ConfirmationContext()
        {
            Action = action,
            Purpose = purpose
        };

        var parameters = new DialogParameters<AccessConfirmationDialog>()
        {
            { dialog => dialog.Context, context }
        };

        var options = new DialogOptions()
        {
            BackdropClick = true,
            CloseOnEscapeKey = true,
            CloseButton = true,
            MaxWidth = MaxWidth.ExtraExtraLarge
        };

        const string title = "Access confirmation";
        var dialog = await dialogService.ShowAsync<AccessConfirmationDialog>(title, parameters, options);
        var result = await dialog.Result;
        return result!;
    }
}
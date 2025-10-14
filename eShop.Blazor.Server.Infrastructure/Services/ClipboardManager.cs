using Microsoft.JSInterop;

namespace eShop.Blazor.Server.Infrastructure.Services;

public class ClipboardManager(IJSRuntime jsRuntime)
{
    private readonly IJSRuntime jsRuntime = jsRuntime;

    public async Task CopyAsync(string text)
    {
        await jsRuntime.InvokeVoidAsync("clipboardCopy", text);
    }
}
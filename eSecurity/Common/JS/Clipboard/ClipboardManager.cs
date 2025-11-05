using Microsoft.JSInterop;

namespace eSecurity.Common.JS.Clipboard;

public class ClipboardManager(IJSRuntime jsRuntime)
{
    private readonly IJSRuntime jsRuntime = jsRuntime;

    public async Task CopyAsync(string text)
    {
        await jsRuntime.InvokeVoidAsync("clipboardCopy", text);
    }
}
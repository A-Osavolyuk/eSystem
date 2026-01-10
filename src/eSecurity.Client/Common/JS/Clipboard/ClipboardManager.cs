using Microsoft.JSInterop;

namespace eSecurity.Client.Common.JS.Clipboard;

public class ClipboardManager(IJSRuntime jsRuntime)
{
    private readonly IJSRuntime _jsRuntime = jsRuntime;

    public async Task CopyAsync(string text)
    {
        await _jsRuntime.InvokeVoidAsync("clipboardCopy", text);
    }
}
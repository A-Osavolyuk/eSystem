using Microsoft.JSInterop;

namespace eSecurity.Client.Common.JS.Print;

public class PrintManager(IJSRuntime jsRuntime)
{
    private readonly IJSRuntime _jsRuntime = jsRuntime;

    public async Task PrintAsync()
    {
        await _jsRuntime.InvokeVoidAsync("triggerPrint");
    }
}
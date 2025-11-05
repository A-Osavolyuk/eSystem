using Microsoft.JSInterop;

namespace eSecurity.Common.JS.Print;

public class PrintManager(IJSRuntime jsRuntime)
{
    private readonly IJSRuntime jsRuntime = jsRuntime;

    public async Task PrintAsync()
    {
        await jsRuntime.InvokeVoidAsync("triggerPrint");
    }
}
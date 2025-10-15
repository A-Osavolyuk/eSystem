using Microsoft.JSInterop;

namespace eShop.Blazor.Server.Infrastructure.Services;

public class PrintManager(IJSRuntime jsRuntime)
{
    private readonly IJSRuntime jsRuntime = jsRuntime;

    public async Task PrintAsync()
    {
        await jsRuntime.InvokeVoidAsync("triggerPrint");
    }
}
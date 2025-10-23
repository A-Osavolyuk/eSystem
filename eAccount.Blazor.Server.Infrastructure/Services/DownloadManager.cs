using Microsoft.JSInterop;

namespace eAccount.Blazor.Server.Infrastructure.Services;

public class DownloadManager(IJSRuntime jSRuntime)
{
    private readonly IJSRuntime jSRuntime = jSRuntime;

    public async Task DownloadAsync(string fileName, object data)
    {
        var json = JsonSerializer.Serialize(data);
        var bytes = Encoding.UTF8.GetBytes(json);
        var base64 = Convert.ToBase64String(bytes);
        
        await jSRuntime.InvokeVoidAsync("downloadFile", fileName, base64);
    }
}
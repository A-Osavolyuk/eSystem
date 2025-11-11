using eSecurity.Client.Common.JS.Clipboard;
using eSecurity.Client.Common.JS.Download;
using eSecurity.Client.Common.JS.Fetch;
using eSecurity.Client.Common.JS.Print;
using eSecurity.Client.Common.JS.WebAuthN;

namespace eSecurity.Client.Common.JS;

public static class JsExtensions
{
    public static void AddJs(this IServiceCollection services)
    {
        services.AddScoped<IFetchClient, FetchClient>();
        services.AddScoped<DownloadManager>();
        services.AddScoped<ClipboardManager>();
        services.AddScoped<PrintManager>();
        services.AddScoped<WebAuthNManager>();
    }
}
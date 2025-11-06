using eSecurity.Common.JS.Clipboard;
using eSecurity.Common.JS.Download;
using eSecurity.Common.JS.Fetch;
using eSecurity.Common.JS.Print;
using eSecurity.Common.JS.WebAuthN;

namespace eSecurity.Common.JS;

public static class JsExtensions
{
    public static void AddJs(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IFetchClient, FetchClient>();
        builder.Services.AddScoped<DownloadManager>();
        builder.Services.AddScoped<ClipboardManager>();
        builder.Services.AddScoped<PrintManager>();
        builder.Services.AddScoped<WebAuthNManager>();
    }
}
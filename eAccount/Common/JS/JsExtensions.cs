using eAccount.Common.JS.Clipboard;
using eAccount.Common.JS.Download;
using eAccount.Common.JS.Fetch;
using eAccount.Common.JS.Print;

namespace eAccount.Common.JS;

public static class JsExtensions
{
    public static void AddJs(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IFetchClient, FetchClient>();
        builder.Services.AddScoped<DownloadManager>();
        builder.Services.AddScoped<ClipboardManager>();
        builder.Services.AddScoped<PrintManager>();
    }
}
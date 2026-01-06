using Microsoft.JSInterop;

namespace eSecurity.Client.Common.JS.Localization;

public class LocalizationManager(IJSRuntime jsRuntime) : ILocalizationManager
{
    private readonly IJSRuntime _jsRuntime = jsRuntime;

    public async ValueTask<string> GetLocaleAsync()
        => await _jsRuntime.InvokeAsync<string>("getLocale");

    public async ValueTask<string> GetTimezoneAsync()
        => await _jsRuntime.InvokeAsync<string>("getTimezone");
}
using eShop.Blazor.Server.Domain.Common;
using eShop.Domain.Common.Security.Credentials;
using Microsoft.JSInterop;

namespace eShop.Blazor.Server.Infrastructure.Security;

public class PasskeyManager(IJSRuntime jsRuntime)
{
    private readonly IJSRuntime jsRuntime = jsRuntime;

    public async Task<JsResult> AssertAsync(PublicKeyCredentialCreationOptions options)
    {
        return await jsRuntime.InvokeAsync<JsResult>("assert", options);
    }
    
    public async Task<JsResult> AuthenticateAsync(PublicKeyCredentialRequestOptions options)
    {
        return await jsRuntime.InvokeAsync<JsResult>("authenticate", options);
    }
}
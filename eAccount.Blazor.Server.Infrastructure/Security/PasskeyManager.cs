using eAccount.Blazor.Server.Domain.Common;
using eSystem.Domain.Security.Credentials.PublicKey;
using Microsoft.JSInterop;

namespace eAccount.Blazor.Server.Infrastructure.Security;

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
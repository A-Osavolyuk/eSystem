using eAccount.Domain.Common;
using eSystem.Core.Security.Credentials.PublicKey;
using Microsoft.JSInterop;

namespace eAccount.Infrastructure.Security;

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
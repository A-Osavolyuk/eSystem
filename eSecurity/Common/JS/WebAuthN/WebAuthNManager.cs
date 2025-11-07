using eSecurity.Common.JS.Fetch;
using eSecurity.Security.Credentials.PublicKey.Credentials;
using Microsoft.JSInterop;

namespace eSecurity.Common.JS.WebAuthN;

public class WebAuthNManager(IJSRuntime jsRuntime)
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
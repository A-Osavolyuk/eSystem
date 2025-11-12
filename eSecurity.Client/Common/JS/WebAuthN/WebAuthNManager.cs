using eSecurity.Client.Common.JS.Fetch;
using eSecurity.Core.Security.Credentials.PublicKey;
using Microsoft.JSInterop;

namespace eSecurity.Client.Common.JS.WebAuthN;

public class WebAuthNManager(IJSRuntime jsRuntime)
{
    private readonly IJSRuntime _jsRuntime = jsRuntime;

    public async Task<JsResult> AssertAsync(PublicKeyCredentialCreationOptions options)
    {
        return await _jsRuntime.InvokeAsync<JsResult>("assert", options);
    }
    
    public async Task<JsResult> AuthenticateAsync(PublicKeyCredentialRequestOptions options)
    {
        return await _jsRuntime.InvokeAsync<JsResult>("authenticate", options);
    }
}
using eShop.Domain.Common.Security.Credentials;
using Microsoft.JSInterop;

namespace eShop.Blazor.Server.Infrastructure.Security;

public class PasskeyManager(IJSRuntime jsRuntime)
{
    private readonly IJSRuntime jsRuntime = jsRuntime;

    public async Task<PublicKeyCredentialCreationResponse> AssertAsync(PublicKeyCredentialCreationOptions options)
    {
        var response = await jsRuntime.InvokeAsync<PublicKeyCredentialCreationResponse>("assert", options);
        return response;
    }
    
    public async Task<PublicKeyCredential> AuthenticateAsync(PublicKeyCredentialRequestOptions options)
    {
        var credential = await jsRuntime.InvokeAsync<PublicKeyCredential>("authenticate", options);
        return credential;
    }
}
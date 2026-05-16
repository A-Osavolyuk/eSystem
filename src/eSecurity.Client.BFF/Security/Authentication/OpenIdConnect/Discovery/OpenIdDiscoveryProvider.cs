using eSecurity.Client.BFF.Security.Authentication.OpenIdConnect.Discovery;
using eSystem.Core.Common.Gateway;
using eSystem.Core.Security.Authentication.OpenIdConnect.Discovery;
using eSystem.Core.Security.Authorization.OAuth;
using Microsoft.Extensions.Options;

namespace eSecurity.Client.BFF.Security.Authentication.OpenIdConnect;

public sealed class OpenIdDiscoveryProvider(
    IHttpClientFactory httpClientFactory, 
    IOptions<OAuthOptions> oauthOptions, 
    GatewayOptions gatewayOptions) : IOpenIdDiscoveryProvider
{
    private readonly HttpClient _client = httpClientFactory.CreateClient("oidc");
    private readonly OAuthOptions _oauthOptions = oauthOptions.Value;
    private readonly GatewayOptions _gatewayOptions = gatewayOptions;
    
    public async ValueTask<OpenIdConfiguration?> GetOpenIdDiscoveryAsync(CancellationToken cancellationToken = default)
    {
        var discoveryUri = $"{_gatewayOptions.Url}{_oauthOptions.Authority}/.well-known/openid-configuration";
        return await _client.GetFromJsonAsync<OpenIdConfiguration>(discoveryUri, cancellationToken);
    }

    public async ValueTask<JsonWebKeySet?> GetJsonWebKeySetAsync(CancellationToken cancellationToken = default)
    {
        var discovery = await GetOpenIdDiscoveryAsync(cancellationToken);
        if (discovery is null) return null;

        return await _client.GetFromJsonAsync<JsonWebKeySet>(discovery.JwksUri, cancellationToken);
    }
}
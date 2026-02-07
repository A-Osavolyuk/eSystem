using eCinema.Server.Security.Authentication.OpenIdConnect.Discovery;
using eSystem.Core.Common.Gateway;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Security.Authorization.OAuth.Constants;
using eSystem.Core.Security.Authorization.OAuth.Token.RefreshToken;
using eSystem.Core.Security.Cryptography.Encoding;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace eCinema.Server.Security.Authentication.OpenIdConnect.Token;

public class TokenHandler (
    IHttpContextAccessor httpContextAccessor,
    IHttpClientFactory httpClientFactory,
    IOpenIdDiscoveryProvider discoveryProvider,
    IOptions<OAuthOptions> options,
    GatewayOptions gatewayOptions) : ITokenHandler 
{
    private readonly IOpenIdDiscoveryProvider _discoveryProvider = discoveryProvider;
    private readonly GatewayOptions _gatewayOptions = gatewayOptions;
    private readonly OAuthOptions _oauthOptions = options.Value;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("gateway");

    public async ValueTask<string?> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        var authResult = await _httpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        var accessToken = await _httpContext.GetTokenAsync("access_token");
        var refreshToken = await _httpContext.GetTokenAsync("refresh_token");
        var expirationString = await _httpContext.GetTokenAsync("expires_at");
        
        if (DateTimeOffset.TryParse(expirationString, out var expiresAt) 
            && expiresAt > DateTime.UtcNow.AddSeconds(30)
            && !string.IsNullOrEmpty(accessToken))
        {
            return accessToken;
        }
        
        if (string.IsNullOrEmpty(refreshToken))
            throw new Exception("No refresh token available to refresh access token.");

        var discovery = await _discoveryProvider.GetOpenIdDiscoveryAsync(cancellationToken);
        if (discovery is null) return null;

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, discovery.TokenEndpoint);
        var content = FormUrl.Encode(new RefreshTokenRequest
        {
            GrantType = GrantTypes.RefreshToken,
            ClientId = _oauthOptions.ClientId,
            RefreshToken = refreshToken,
        });
        
        requestMessage.Content = new FormUrlEncodedContent(content);
        requestMessage.Headers.WithBasicAuthentication(_oauthOptions.ClientId, _oauthOptions.ClientSecret);

        var response = await _httpClient.SendAsync(requestMessage, cancellationToken);
        response.EnsureSuccessStatusCode();

        var tokenResponse = await response.Content.ReadFromJsonAsync<RefreshTokenResponse>(cancellationToken);
        
        authResult.Properties?.UpdateTokenValue("access_token", tokenResponse?.AccessToken);
        authResult.Properties?.UpdateTokenValue("refresh_token", tokenResponse?.RefreshToken);
        authResult.Properties?.UpdateTokenValue("expires_at", 
            DateTime.UtcNow.AddSeconds(tokenResponse!.ExpiresIn).ToString("o"));
        
        await _httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, 
            authResult.Principal!, 
            authResult.Properties
            );

        return tokenResponse?.AccessToken;
    }
}
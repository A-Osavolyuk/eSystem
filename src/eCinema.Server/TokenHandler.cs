using eSystem.Core.Common.Gateway;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Security.Authentication.Oidc;
using eSystem.Core.Security.Authentication.Oidc.Constants;
using eSystem.Core.Security.Authentication.Oidc.Token;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Security.Cryptography.Encoding;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace eCinema.Server;

public class TokenHandler(
    IHttpContextAccessor httpContextAccessor,
    IHttpClientFactory httpClientFactory,
    IOptions<OAuthOptions> options,
    GatewayOptions gatewayOptions)
{
    private readonly GatewayOptions _gatewayOptions = gatewayOptions;
    private readonly OAuthOptions _oAuthOptions = options.Value;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("gateway");

    public async Task<string?> GetTokenAsync()
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

        var discoveryUri = $"{_gatewayOptions.Url}{_oAuthOptions.Authority}/.well-known/openid-configuration";
        var discovery = await _httpClient.GetFromJsonAsync<OpenIdConfiguration>(discoveryUri);

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, discovery!.TokenEndpoint);
        var content = FormUrl.Encode(new TokenRequest
        {
            GrantType = GrantTypes.RefreshToken,
            ClientId = _oAuthOptions.ClientId,
            RefreshToken = refreshToken,
        });
        
        requestMessage.Content = new FormUrlEncodedContent(content);
        requestMessage.Headers.WithBasicAuthentication(_oAuthOptions.ClientId, _oAuthOptions.ClientSecret);

        var response = await _httpClient.SendAsync(requestMessage);
        response.EnsureSuccessStatusCode();

        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
        
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
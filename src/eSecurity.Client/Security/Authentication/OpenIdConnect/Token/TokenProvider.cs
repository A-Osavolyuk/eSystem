using System.Text.Json;
using eSecurity.Client.Common.JS.Fetch;
using eSecurity.Client.Security.Cookies.Constants;
using eSecurity.Client.Security.Cryptography.Protection.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Client.Security.Authentication.OpenIdConnect.Token;

public class TokenProvider(
    IDataProtectionProvider protectionProvider,
    IHttpContextAccessor httpContextAccessor,
    IFetchClient fetchClient,
    NavigationManager navigationManager) : ITokenProvider
{
    private readonly IFetchClient _fetchClient = fetchClient;
    private readonly NavigationManager _navigationManager = navigationManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly IDataProtector _protector = protectionProvider.CreateProtector(ProtectionPurposes.Token);

    public string? Get(string key)
    {
        var cookie = _httpContext.Request.Cookies[DefaultCookies.Payload];
        if (string.IsNullOrEmpty(cookie)) return null;
        
        var cookieJson = _protector.Unprotect(cookie);
        var authenticationMetadata = JsonSerializer.Deserialize<AuthenticationMetadata>(cookieJson);
        return authenticationMetadata?.Tokens.FirstOrDefault(x => x.Name == key)?.Value;
    }
    
    public async Task SetAsync(AuthenticationMetadata metadata,
        CancellationToken cancellationToken = default)
    {
        var fetchOptions = new FetchOptions
        {
            Body = metadata,
            Method = HttpMethod.Post,
            Url = $"{_navigationManager.BaseUri}api/authentication/refresh"
        };
        
        await _fetchClient.FetchAsync(fetchOptions);
    }
}
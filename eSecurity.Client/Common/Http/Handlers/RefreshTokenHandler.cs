using System.Net;
using System.Text.Json;
using eSecurity.Client.Common.Http.Extensions;
using eSecurity.Client.Common.JS.Fetch;
using eSecurity.Client.Security.Authentication.Oidc.Clients;
using eSecurity.Client.Security.Authentication.Oidc.Token;
using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSecurity.Core.Common.Routing;
using eSecurity.Core.Security.Cookies;
using eSecurity.Core.Security.Cookies.Constants;
using eSystem.Core.Common.Http.Context;
using eSystem.Core.Common.Network.Gateway;
using eSystem.Core.Security.Authentication.Oidc.Token;
using eSystem.Core.Security.Cryptography.Encoding;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace eSecurity.Client.Common.Http.Handlers;

public class RefreshTokenHandler(
    ICookieAccessor cookieAccessor,
    IFetchClient fetchClient,
    IOptions<ClientOptions> clientOptions,
    IOptions<GatewayOptions> gatewayOptions,
    IHttpContextAccessor httpContextAccessor,
    IHttpClientFactory httpClientFactory,
    TokenProvider tokenProvider,
    NavigationManager navigationManager) : DelegatingHandler
{
    private readonly ICookieAccessor _cookieAccessor = cookieAccessor;
    private readonly IFetchClient _fetchClient = fetchClient;
    private readonly TokenProvider _tokenProvider = tokenProvider;
    private readonly NavigationManager _navigationManager = navigationManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly ClientOptions _clientOptions = clientOptions.Value;
    private readonly GatewayOptions _gatewayOptions = gatewayOptions.Value;
    private readonly HttpClient _rawClient = httpClientFactory.CreateClient("Raw");

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
        
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            if (request.Headers.Contains(HeaderTypes.XRetry))
                return response;
            
            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var error = JsonSerializer.Deserialize<Error>(responseJson);
            
            if (error is not null && error.Code == Errors.OAuth.InvalidToken)
            {
                var requestMessage = new HttpRequestMessage();
                requestMessage.Headers.WithBasicAuthentication(_clientOptions.ClientId, _clientOptions.ClientSecret);
                requestMessage.Method = HttpMethod.Post;
                requestMessage.RequestUri = new Uri($"{_gatewayOptions.Url}/api/v1/Connect/token");
                requestMessage.Headers.WithCookies(_httpContext.GetCookies());
                requestMessage.Headers.WithUserAgent(_httpContext.GetUserAgent());
                
                var refreshToken = _cookieAccessor.Get(DefaultCookies.RefreshToken);
                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    _tokenProvider.Clear();
                    _navigationManager.NavigateTo(Links.Account.SignIn);
                    return response;
                }
                
                var content = FormUrl.Encode(new TokenRequest()
                {
                    ClientId = _clientOptions.ClientId,
                    ClientSecret = _clientOptions.ClientSecret,
                    RefreshToken = refreshToken,
                    GrantType = GrantTypes.RefreshToken
                });
                
                requestMessage.Headers.Add(HeaderTypes.Accept, ContentTypes.Application.XwwwFormUrlEncoded);
                requestMessage.Content = new FormUrlEncodedContent(content);
                
                var tokenResult = await _rawClient.SendAsync(requestMessage, cancellationToken);
                if (!tokenResult.IsSuccessStatusCode) return response;
                
                var tokenResponseJson = await tokenResult.Content.ReadAsStringAsync(cancellationToken);
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(tokenResponseJson);
                if (tokenResponse is null) return response;
                
                _tokenProvider.AccessToken = tokenResponse.AccessToken;
                _tokenProvider.IdToken = tokenResponse.IdToken;
                
                var fetchOptions = new FetchOptions()
                {
                    Method = HttpMethod.Post,
                    Url = $"{_navigationManager.BaseUri}api/authentication/refresh",
                    Body = tokenResponse.RefreshToken
                };
                
                var fetchResponse = await _fetchClient.FetchAsync(fetchOptions);
                if (!fetchResponse.Succeeded)
                {
                    _tokenProvider.Clear();
                    _navigationManager.NavigateTo(Links.Account.SignIn);
                    return response;
                }

                var retry = await CloneAsync(request, cancellationToken);
                return await base.SendAsync(retry, cancellationToken);
            }
        }

        return response;
    }

    private async Task<HttpRequestMessage> CloneAsync(HttpRequestMessage request, 
        CancellationToken cancellationToken = default)
    {
        var retryRequestMessage = new HttpRequestMessage(request.Method, request.RequestUri);

        foreach (var header in request.Headers.Where(
                     x => x.Key != HeaderTypes.Authorization))
        {
            retryRequestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
                
        if (request.Content != null)
        {
            var bytes = await request.Content.ReadAsByteArrayAsync(cancellationToken);
            retryRequestMessage.Content = new ByteArrayContent(bytes);

            foreach (var header in request.Content.Headers)
                retryRequestMessage.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
        
        retryRequestMessage.Headers.Add(HeaderTypes.XRetry, "1");
        retryRequestMessage.Headers.WithBearerAuthentication(_tokenProvider.AccessToken);
        
        return retryRequestMessage;
    }
}
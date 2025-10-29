using System.Net;
using eAccount.Domain.Constants;
using eAccount.Domain.Options;
using eAccount.Infrastructure.Http;
using eAccount.Infrastructure.Http.Extensions;
using eAccount.Infrastructure.Security.Authentication.JWT;
using eAccount.Infrastructure.Security.Authentication.SSO.Clients;
using eSystem.Core.Common.Network.Gateway;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Responses.Auth;
using eSystem.Core.Security.Authentication.Cookies;
using eSystem.Core.Security.Authentication.SSO.Token;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using HttpRequest = eSystem.Core.Common.Http.HttpRequest;
using HttpResponse = eSystem.Core.Common.Http.HttpResponse;

namespace eAccount.Infrastructure.Implementations;

public class ApiClient(
    IHttpClientFactory clientFactory,
    IHttpContextAccessor httpContextAccessor,
    IFetchClient fetchClient,
    ICookieAccessor cookieAccessor,
    IOptions<ClientOptions> options,
    GatewayOptions gatewayOptions,
    TokenProvider tokenProvider,
    NavigationManager navigationManager) : IApiClient
{
    private readonly IHttpClientFactory clientFactory = clientFactory;
    private readonly TokenProvider tokenProvider = tokenProvider;
    private readonly IFetchClient fetchClient = fetchClient;
    private readonly ICookieAccessor cookieAccessor = cookieAccessor;
    private readonly IOptions<ClientOptions> options = options;
    private readonly GatewayOptions gatewayOptions = gatewayOptions;
    private readonly NavigationManager navigationManager = navigationManager;
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext!;

    public async ValueTask<HttpResponse> SendAsync(HttpRequest httpRequest, HttpOptions httpOptions)
    {
        try
        {
            var message = new HttpRequestMessage();
            
            message.Headers.AddBearerAuthorization(tokenProvider.AccessToken);
            message.RequestUri = new Uri($"{gatewayOptions.Url}/{httpRequest.Url}");
            message.Method = httpRequest.Method;
            message.IncludeUserAgent(httpContext);
            message.IncludeCookies(httpContext);
            message.AddContent(httpRequest, httpOptions);

            var httpClient = clientFactory.CreateClient("eAccount.Client");
            var httpResponseMessage = await httpClient.SendAsync(message);

            if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
            {
                var result = await RefreshAsync();
                
                if (!result.Success)
                {
                    tokenProvider.Clear();
                    navigationManager.NavigateTo(Links.LoginPage);
                }
                else
                {
                    message.Headers.AddBearerAuthorization(tokenProvider.AccessToken);
                    httpResponseMessage = await httpClient.SendAsync(message);
                }
            }
            
            var serializationOptions = new JsonSerializerOptions()
            {
                WriteIndented = true, 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            return await httpResponseMessage.Content.ReadAsAsync<HttpResponse>(serializationOptions);
        }
        catch (Exception ex)
        {
            var response = new ResponseBuilder()
                .Failed()
                .WithMessage(ex.Message)
                .Build();

            return response;
        }
    }
    
    private async Task<HttpResponse> RefreshAsync()
    {
        var clientOptions = options.Value;
        var refreshToken = cookieAccessor.Get(DefaultCookies.RefreshToken)!;
        var tokenRequest = new TokenRequest()
        {
            ClientId = clientOptions.ClientId,
            ClientSecret = clientOptions.ClientSecret,
            RefreshToken = refreshToken,
            GrantType = GrantTypes.RefreshToken
        };
        
        var httpRequest = new HttpRequest()
        {
            Method = HttpMethod.Post,
            Url = $"{gatewayOptions.Url}/api/v1/Sso/token",
            Data = tokenRequest
        };
        
        var httpOptions = new HttpOptions() { Type = DataType.Text };
        var tokenResult = await SendAsync(httpRequest, httpOptions);

        if (!tokenResult.Success) return tokenResult;
        
        var tokenResponse = tokenResult.Get<TokenResponse>()!;
        tokenProvider.AccessToken = tokenResponse.AccessToken;
            
        var refreshRequest = new RefreshRequest() { RefreshToken = tokenResponse.RefreshToken };
        var fetchOptions = new FetchOptions()
        {
            Method = HttpMethod.Post,
            Url = $"{navigationManager.BaseUri}api/sso/refresh",
            Body = refreshRequest
        };

        return await fetchClient.FetchAsync(fetchOptions);

    }
}
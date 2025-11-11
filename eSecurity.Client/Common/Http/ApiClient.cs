using System.Net;
using System.Text.Json;
using eSecurity.Client.Common.Http.Extensions;
using eSecurity.Client.Common.JS.Fetch;
using eSecurity.Client.Security.Authentication;
using eSecurity.Client.Security.Authentication.Odic;
using eSecurity.Client.Security.Authentication.Odic.Clients;
using eSecurity.Client.Security.Authentication.Odic.Token;
using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSecurity.Core.Common.Routing;
using eSecurity.Core.Security.Cookies;
using eSecurity.Core.Security.Cookies.Constants;
using eSystem.Core.Common.Http;
using eSystem.Core.Common.Network.Gateway;
using eSystem.Core.Security.Authentication.Odic.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using AuthenticationManager = eSecurity.Client.Security.Authentication.AuthenticationManager;

namespace eSecurity.Client.Common.Http;

public class ApiClient(
    IHttpClientFactory clientFactory,
    IHttpContextAccessor httpContextAccessor,
    ICookieAccessor cookieAccessor,
    IFetchClient fetchClient,
    NavigationManager navigationManager,
    TokenProvider tokenProvider,
    AuthenticationManager authenticationManager,
    IOptions<GatewayOptions> gatewayOptions,
    IOptions<ClientOptions> clientOptions) : IApiClient
{
    private readonly IHttpClientFactory clientFactory = clientFactory;
    private readonly ICookieAccessor cookieAccessor = cookieAccessor;
    private readonly IFetchClient fetchClient = fetchClient;
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext!;
    private readonly NavigationManager navigationManager = navigationManager;
    private readonly TokenProvider tokenProvider = tokenProvider;
    private readonly AuthenticationManager authenticationManager = authenticationManager;
    private readonly GatewayOptions gatewayOptions = gatewayOptions.Value;
    private readonly ClientOptions clientOptions = clientOptions.Value;

    public async ValueTask<HttpResponse> SendAsync(HttpRequest httpRequest, HttpOptions httpOptions)
    {
        try
        {
            var message = new HttpRequestMessage();
            
            if (httpOptions.WithBearer)
            {
                if (string.IsNullOrEmpty(tokenProvider.AccessToken))
                {
                    var result = await RefreshAsync();
                
                    if (!result.Success)
                    {
                        tokenProvider.Clear();
                        navigationManager.NavigateTo(Links.Account.SignIn);
                    }
                }
                
                message.Headers.AddBearerAuthorization(tokenProvider.AccessToken);
            }
            
            message.RequestUri = new Uri($"{gatewayOptions.Url}/{httpRequest.Url}");
            message.Method = httpRequest.Method;
            message.IncludeUserAgent(httpContext);
            message.IncludeCookies(httpContext);
            message.AddContent(httpRequest, httpOptions);

            var httpClient = clientFactory.CreateClient("eSecurity.Client");
            var httpResponseMessage = await httpClient.SendAsync(message);

            if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
            {
                var result = await RefreshAsync();
                
                if (!result.Success)
                {
                    await authenticationManager.SignOutAsync();
                    navigationManager.NavigateTo(Links.Account.SignIn);
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
            
            var response = await httpResponseMessage.Content.ReadAsAsync<HttpResponse>(serializationOptions);
            return response;
        }
        catch (Exception ex)
        {
            var response = HttpResponseBuilder.Create()
                .Failed()
                .WithMessage(ex.Message)
                .Build();

            return response;
        }
    }
    
    private async Task<HttpResponse> RefreshAsync()
    {
        var refreshToken = cookieAccessor.Get(DefaultCookies.RefreshToken)!;
        var request = new TokenRequest()
        {
            ClientId = clientOptions.ClientId,
            ClientSecret = clientOptions.ClientSecret,
            RefreshToken = refreshToken,
            GrantType = GrantTypes.RefreshToken
        };
        
        var httpRequest = new HttpRequest()
        {
            Method = HttpMethod.Post,
            Url = "api/v1/Connect/token",
            Data = request
        };
        
        var httpOptions = new HttpOptions() { Type = DataType.Text };
        var result = await SendAsync(httpRequest, httpOptions);

        if (!result.Success) return result;
        
        var response = result.Get<TokenResponse>()!;
        tokenProvider.AccessToken = response.AccessToken;
        tokenProvider.IdToken = response.IdToken;

        var fetchOptions = new FetchOptions()
        {
            Method = HttpMethod.Post,
            Url = $"{navigationManager.BaseUri}api/authentication/refresh",
            Body = refreshToken
        };
        
        return await fetchClient.FetchAsync(fetchOptions);
    }
}

using System.Net;
using System.Text.Json;
using eAccount.Common.Routing;
using eAccount.Security.Authentication.JWT;
using eAccount.Security.Cookies;
using eAccount.Common.Http.Extensions;
using eAccount.Security.Authentication.ODIC.Clients;
using eSystem.Core.Common.Http;
using eSystem.Core.Common.Network.Gateway;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Responses.Auth;
using eSystem.Core.Security.Authentication.ODIC.Constants;
using eSystem.Core.Security.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace eAccount.Common.Http;

public class ApiClient(
    IHttpClientFactory clientFactory,
    IHttpContextAccessor httpContextAccessor,
    ICookieAccessor cookieAccessor,
    IOptions<ClientOptions> options,
    GatewayOptions gatewayOptions,
    TokenProvider tokenProvider,
    NavigationManager navigationManager,
    AuthenticationManager authenticationManager) : IApiClient
{
    private readonly IHttpClientFactory clientFactory = clientFactory;
    private readonly TokenProvider tokenProvider = tokenProvider;
    private readonly ICookieAccessor cookieAccessor = cookieAccessor;
    private readonly IOptions<ClientOptions> options = options;
    private readonly GatewayOptions gatewayOptions = gatewayOptions;
    private readonly NavigationManager navigationManager = navigationManager;
    private readonly AuthenticationManager authenticationManager = authenticationManager;
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext!;

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
                        navigationManager.NavigateTo(Links.SignIn);
                    }
                }
                
                message.Headers.AddBearerAuthorization(tokenProvider.AccessToken);
            }
            
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
                    await authenticationManager.SignOutAsync();
                    navigationManager.NavigateTo(Links.SignIn);
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
        var clientOptions = options.Value;
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
            Url = "api/v1/Sso/token",
            Data = request
        };
        
        var httpOptions = new HttpOptions() { Type = DataType.Text };
        var result = await SendAsync(httpRequest, httpOptions);

        if (!result.Success) return result;
        
        var response = result.Get<TokenResponse>()!;
        tokenProvider.AccessToken = response.AccessToken;

        return await authenticationManager.RefreshAsync(response.RefreshToken!);

    }
}
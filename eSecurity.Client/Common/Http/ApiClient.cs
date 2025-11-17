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
using eSystem.Core.Common.Http;
using eSystem.Core.Common.Network.Gateway;
using eSystem.Core.Security.Authentication.Oidc.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using AuthenticationManager = eSecurity.Client.Security.Authentication.AuthenticationManager;
using Results = eSystem.Core.Common.Results.Results;

namespace eSecurity.Client.Common.Http;

public class ApiClient(
    IHttpClientFactory clientFactory,
    IHttpContextAccessor httpContextAccessor,
    ICookieAccessor cookieAccessor,
    IFetchClient fetchClient,
    NavigationManager navigationManager,
    TokenProvider tokenProvider,
    AuthenticationManager authenticationManager,
    GatewayOptions gatewayOptions,
    IOptions<ClientOptions> clientOptions) : IApiClient
{
    private readonly IHttpClientFactory _clientFactory = clientFactory;
    private readonly ICookieAccessor _cookieAccessor = cookieAccessor;
    private readonly IFetchClient _fetchClient = fetchClient;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly NavigationManager _navigationManager = navigationManager;
    private readonly TokenProvider _tokenProvider = tokenProvider;
    private readonly AuthenticationManager _authenticationManager = authenticationManager;
    private readonly GatewayOptions _gatewayOptions = gatewayOptions;
    private readonly ClientOptions _clientOptions = clientOptions.Value;

    public async ValueTask<Result> SendAsync(HttpRequest httpRequest, HttpOptions httpOptions)
    {
        try
        {
            var message = new HttpRequestMessage();

            if (httpOptions.WithBearer)
            {
                if (string.IsNullOrEmpty(_tokenProvider.AccessToken))
                {
                    var result = await RefreshAsync();

                    if (!result.Succeeded)
                    {
                        _tokenProvider.Clear();
                        _navigationManager.NavigateTo(Links.Account.SignIn);
                    }
                }

                message.Headers.AddBearerAuthorization(_tokenProvider.AccessToken);
            }

            message.RequestUri = new Uri($"{_gatewayOptions.Url}/{httpRequest.Url}");
            message.Method = httpRequest.Method;
            message.IncludeUserAgent(_httpContext);
            message.IncludeCookies(_httpContext);
            message.AddContent(httpRequest, httpOptions);

            var httpClient = _clientFactory.CreateClient("eSecurity.Client");
            var httpResponseMessage = await httpClient.SendAsync(message);

            var serializationOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                return Result.Success(httpResponseMessage.StatusCode, content);
            }
            else
            {
                var error = await httpResponseMessage.Content.ReadAsAsync<Error>(serializationOptions);
                return Result.Failure(httpResponseMessage.StatusCode, error);
            }
        }
        catch (Exception ex)
        {
            return Results.InternalServerError(ex.Message);
        }
    }

    private async Task<Result> RefreshAsync()
    {
        var refreshToken = _cookieAccessor.Get(DefaultCookies.RefreshToken)!;
        var request = new TokenRequest()
        {
            ClientId = _clientOptions.ClientId,
            ClientSecret = _clientOptions.ClientSecret,
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

        if (!result.Succeeded) return result;

        var response = result.Get<TokenResponse>();
        _tokenProvider.AccessToken = response.AccessToken;
        _tokenProvider.IdToken = response.IdToken;

        var fetchOptions = new FetchOptions()
        {
            Method = HttpMethod.Post,
            Url = $"{_navigationManager.BaseUri}api/authentication/refresh",
            Body = refreshToken
        };

        return await _fetchClient.FetchAsync(fetchOptions);
    }
}
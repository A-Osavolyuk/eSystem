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
using eSystem.Core.Security.Authentication.Oidc.Constants;
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
    GatewayOptions gatewayOptions,
    IOptions<ClientOptions> clientOptions) : IApiClient
{
    private readonly IHttpClientFactory _clientFactory = clientFactory;
    private readonly ICookieAccessor _cookieAccessor = cookieAccessor;
    private readonly IFetchClient _fetchClient = fetchClient;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly NavigationManager _navigationManager = navigationManager;
    private readonly TokenProvider _tokenProvider = tokenProvider;
    private readonly GatewayOptions _gatewayOptions = gatewayOptions;
    private readonly ClientOptions _clientOptions = clientOptions.Value;

    public async ValueTask<HttpResponse<TResponse>> SendAsync<TResponse>(
        HttpRequest httpRequest, HttpOptions httpOptions)
    {
        try
        {
            var httpResponseMessage = await SendRequestAsync(httpRequest, httpOptions);
            var serializationOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var content = await httpResponseMessage.Content.ReadAsAsync<TResponse>(serializationOptions);
                return HttpResponse<TResponse>.Success(content);
            }

            var error = await httpResponseMessage.Content.ReadAsAsync<Error>(serializationOptions);
            return HttpResponse<TResponse>.Fail(error);
        }
        catch (Exception ex)
        {
            return HttpResponse<TResponse>.Fail(new Error()
            {
                Code = Errors.Common.InternalServerError,
                Description = ex.Message
            });
        }
    }

    public async ValueTask<HttpResponse> SendAsync(HttpRequest httpRequest, HttpOptions httpOptions)
    {
        try
        {
            var httpResponseMessage = await SendRequestAsync(httpRequest, httpOptions);
            var serializationOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return HttpResponse.Success();
            }

            var error = await httpResponseMessage.Content.ReadAsAsync<Error>(serializationOptions);
            return HttpResponse.Fail(error);
        }
        catch (Exception ex)
        {
            return HttpResponse.Fail(new Error()
            {
                Code = Errors.Common.InternalServerError,
                Description = ex.Message
            });
        }
    }

    private async ValueTask<HttpResponseMessage> SendRequestAsync(HttpRequest httpRequest, HttpOptions httpOptions)
    {
        var message = new HttpRequestMessage();

        if (httpOptions.Authentication == AuthenticationType.Bearer)
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

            message.Headers.WithBearerAuthentication(_tokenProvider.AccessToken);
        }
        else if (httpOptions.Authentication == AuthenticationType.Basic)
        {
            message.Headers.WithBasicAuthentication(_clientOptions.ClientId, _clientOptions.ClientSecret);
        }

        message.RequestUri = new Uri($"{_gatewayOptions.Url}/{httpRequest.Url}");
        message.Method = httpRequest.Method;
        message.IncludeUserAgent(_httpContext);
        message.IncludeCookies(_httpContext);
        message.AddContent(httpRequest, httpOptions);

        var httpClient = _clientFactory.CreateClient("eSecurity.Client");
        return await httpClient.SendAsync(message);
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


        var httpOptions = new HttpOptions()
        {
            ContentType = ContentTypes.Application.Json,
            Authentication = AuthenticationType.Basic
        };

        var result = await SendAsync<TokenResponse>(httpRequest, httpOptions);

        if (!result.Succeeded)
        {
            var error = result.GetError();
            return Results.BadRequest(new Error()
            {
                Code = error.Code,
                Description = error.Description,
            });
        }

        var response = result.Get();
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
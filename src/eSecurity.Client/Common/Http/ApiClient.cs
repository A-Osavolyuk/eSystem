using System.Net;
using System.Text.Json;
using eSecurity.Client.Common.Http.Extensions;
using eSecurity.Client.Common.JS.Fetch;
using eSecurity.Client.Common.JS.Localization;
using eSecurity.Client.Security.Authentication.Oidc.Token;
using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSecurity.Core.Common.Routing;
using eSecurity.Core.Security.Cookies;
using eSecurity.Core.Security.Cookies.Constants;
using eSystem.Core.Common.Http.Context;
using eSystem.Core.Common.Network.Gateway;
using eSystem.Core.Security.Authentication.Oidc.Client;
using eSystem.Core.Security.Authentication.Oidc.Token;
using eSystem.Core.Security.Cryptography.Encoding;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace eSecurity.Client.Common.Http;

public class ApiClient(
    HttpClient httpClient,
    TokenProvider tokenProvider,
    GatewayOptions gatewayOptions,
    NavigationManager navigationManager,
    ICookieAccessor cookieAccessor,
    IFetchClient fetchClient,
    IOptions<ClientOptions> clientOptions,
    ILocalizationManager localizationManager,
    IHttpContextAccessor httpContextAccessor) : IApiClient
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly TokenProvider _tokenProvider = tokenProvider;
    private readonly GatewayOptions _gatewayOptions = gatewayOptions;
    private readonly NavigationManager _navigationManager = navigationManager;
    private readonly ICookieAccessor _cookieAccessor = cookieAccessor;
    private readonly IFetchClient _fetchClient = fetchClient;
    private readonly ILocalizationManager _localizationManager = localizationManager;
    private readonly ClientOptions _clientOptions = clientOptions.Value;

    private readonly JsonSerializerOptions _serializationOptions = new JsonSerializerOptions()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async ValueTask<HttpResponse<TResponse>> SendAsync<TResponse>(
        HttpRequest httpRequest, 
        HttpOptions httpOptions, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var httpResponseMessage = await SendRequestAsync(httpRequest, httpOptions, cancellationToken);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var content = await httpResponseMessage.Content.ReadAsync<TResponse>(_serializationOptions);
                return HttpResponse<TResponse>.Success(content);
            }

            var error = await httpResponseMessage.Content.ReadAsync<Error>(_serializationOptions);
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

    public async ValueTask<HttpResponse> SendAsync(
        HttpRequest httpRequest, 
        HttpOptions httpOptions, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var httpResponseMessage = await SendRequestAsync(httpRequest, httpOptions, cancellationToken);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return HttpResponse.Success();
            }

            var error = await httpResponseMessage.Content.ReadAsync<Error>(_serializationOptions);
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

    private async ValueTask<HttpResponseMessage> SendRequestAsync(
        HttpRequest httpRequest, 
        HttpOptions httpOptions, 
        CancellationToken cancellationToken = default)
    {
        var requestMessage = await InitializeAsync(httpRequest, httpOptions);
        var responseMessage = await _httpClient.SendAsync(requestMessage, cancellationToken);

        if (responseMessage.StatusCode != HttpStatusCode.Unauthorized)
            return responseMessage;

        var responseJson = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
        var error = JsonSerializer.Deserialize<Error>(responseJson);

        if (error is null || error.Code != Errors.OAuth.InvalidToken)
            return responseMessage;

        return await RefreshAsync(requestMessage, responseMessage, cancellationToken);
    }

    private async Task<HttpRequestMessage> InitializeAsync(HttpRequest httpRequest, HttpOptions httpOptions)
    {
        var message = new HttpRequestMessage();

        if (httpOptions.Authentication == AuthenticationType.Bearer)
        {
            message.Headers.WithBearerAuthentication(_tokenProvider.AccessToken);
        }
        else if (httpOptions.Authentication == AuthenticationType.Basic)
        {
            message.Headers.WithBasicAuthentication(_clientOptions.ClientId, _clientOptions.ClientSecret);
        }

        if (httpOptions.WithTimezone)
            message.Headers.WithTimeZone(await _localizationManager.GetTimeZoneAsync());

        if (httpOptions.WithLocale)
            message.Headers.WithLocale(await _localizationManager.GetLocaleAsync());

        message.RequestUri = new Uri($"{_gatewayOptions.Url}/{httpRequest.Url}");
        message.Method = httpRequest.Method;
        message.Headers.WithUserAgent(_httpContext.GetUserAgent());
        message.Headers.WithCookies(_httpContext.GetCookies());
        message.AddContent(httpRequest, httpOptions);

        return message;
    }

    private async Task<HttpResponseMessage> RefreshAsync(
        HttpRequestMessage requestMessage,
        HttpResponseMessage responseMessage,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage();
        request.Headers.WithBasicAuthentication(_clientOptions.ClientId, _clientOptions.ClientSecret);
        request.Method = HttpMethod.Post;
        request.RequestUri = new Uri($"{_gatewayOptions.Url}/api/v1/Connect/token");
        request.Headers.WithCookies(_httpContext.GetCookies());
        request.Headers.WithUserAgent(_httpContext.GetUserAgent());

        var content = FormUrl.Encode(new TokenRequest()
        {
            ClientId = _clientOptions.ClientId,
            ClientSecret = _clientOptions.ClientSecret,
            RefreshToken = _cookieAccessor.Get(DefaultCookies.RefreshToken),
            GrantType = GrantTypes.RefreshToken
        });

        request.Headers.Add(HeaderTypes.Accept, ContentTypes.Application.XwwwFormUrlEncoded);
        request.Content = new FormUrlEncodedContent(content);

        var tokenResult = await _httpClient.SendAsync(request, cancellationToken);
        if (!tokenResult.IsSuccessStatusCode) return responseMessage;

        var tokenResponseJson = await tokenResult.Content.ReadAsStringAsync(cancellationToken);
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(tokenResponseJson);
        if (tokenResponse is null) return responseMessage;

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
            _navigationManager.NavigateTo(Links.Common.SignIn);
            return responseMessage;
        }

        var retry = await CloneAsync(requestMessage, cancellationToken);
        return await _httpClient.SendAsync(retry, cancellationToken);
    }

    private async Task<HttpRequestMessage> CloneAsync(HttpRequestMessage request,
        CancellationToken cancellationToken = default)
    {
        var retryRequestMessage = new HttpRequestMessage(request.Method, request.RequestUri);

        var headers = request.Headers.Where(
            x => x.Key != HeaderTypes.Authorization);
        
        foreach (var header in headers)
            retryRequestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);

        if (request.Content != null)
        {
            var bytes = await request.Content.ReadAsByteArrayAsync(cancellationToken);
            retryRequestMessage.Content = new ByteArrayContent(bytes);

            foreach (var header in request.Content.Headers)
                retryRequestMessage.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
        
        retryRequestMessage.Headers.WithBearerAuthentication(_tokenProvider.AccessToken);

        return retryRequestMessage;
    }
}
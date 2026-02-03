using System.Net;
using System.Text.Json;
using eSecurity.Client.Common.JS.Localization;
using eSecurity.Client.Security.Authentication;
using eSecurity.Client.Security.Authentication.OpenIdConnect.Token;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Security.Authorization.OAuth.Constants;
using eSystem.Core.Security.Authorization.OAuth.Token;
using eSystem.Core.Security.Cryptography.Encoding;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace eSecurity.Client.Common.Http;

public class ApiClient(
    HttpClient httpClient,
    ITokenProvider tokenProvider,
    IOptions<ClientOptions> clientOptions,
    ILocalizationManager localizationManager,
    IHttpContextAccessor httpContextAccessor) : IApiClient
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ITokenProvider _tokenProvider = tokenProvider;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly ILocalizationManager _localizationManager = localizationManager;
    private readonly ClientOptions _clientOptions = clientOptions.Value;

    public async ValueTask<ApiResponse> SendAsync(
        ApiRequest request,
        ApiOptions options,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await SendRequestAsync(request, options, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                return ApiResponse.Success(content);
            }

            var serializationOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var error = await response.Content.ReadAsync<Error>(serializationOptions, cancellationToken);
            return ApiResponse.Fail(error);
        }
        catch (Exception ex)
        {
            return ApiResponse.Fail(new Error
            {
                Code = ErrorTypes.Common.InternalServerError,
                Description = ex.Message
            });
        }
    }

    private async ValueTask<HttpResponseMessage> SendRequestAsync(
        ApiRequest apiRequest,
        ApiOptions apiOptions,
        CancellationToken cancellationToken = default)
    {
        var requestMessage = await InitializeAsync(apiRequest, apiOptions);
        var responseMessage = await _httpClient.SendAsync(requestMessage, cancellationToken);

        if (responseMessage.StatusCode != HttpStatusCode.Unauthorized)
            return responseMessage;

        var responseJson = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
        var error = JsonSerializer.Deserialize<Error>(responseJson);

        if (error is null || error.Code != ErrorTypes.OAuth.InvalidToken)
            return responseMessage;

        return await RefreshAsync(requestMessage, responseMessage, cancellationToken);
    }

    private async Task<HttpRequestMessage> InitializeAsync(ApiRequest apiRequest, ApiOptions apiOptions)
    {
        var message = new HttpRequestMessage(apiRequest.Method, apiRequest.Url);
        if (apiOptions.Authentication == AuthenticationType.Bearer)
        {
            var token = _tokenProvider.Get(TokenTypes.AccessToken);
            message.Headers.WithBearerAuthentication(token);
        }
        else if (apiOptions.Authentication == AuthenticationType.Basic)
        {
            message.Headers.WithBasicAuthentication(_clientOptions.ClientId, _clientOptions.ClientSecret);
        }

        if (apiOptions.WithTimezone)
            message.Headers.WithTimeZone(await _localizationManager.GetTimeZoneAsync());

        if (apiOptions.WithLocale)
            message.Headers.WithLocale(await _localizationManager.GetLocaleAsync());

        message.Headers.WithUserAgent(_httpContext.GetUserAgent());
        message.Headers.WithCookies(_httpContext.GetCookies());
        message.AddContent(apiRequest, apiOptions);

        return message;
    }

    private async Task<HttpResponseMessage> RefreshAsync(
        HttpRequestMessage requestMessage,
        HttpResponseMessage responseMessage,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/Connect/token");
        request.Headers.WithBasicAuthentication(_clientOptions.ClientId, _clientOptions.ClientSecret);
        request.Headers.WithCookies(_httpContext.GetCookies());
        request.Headers.WithUserAgent(_httpContext.GetUserAgent());

        var content = FormUrl.Encode(new TokenRequest
        {
            ClientId = _clientOptions.ClientId,
            ClientSecret = _clientOptions.ClientSecret,
            GrantType = GrantTypes.RefreshToken,
            RefreshToken = _tokenProvider.Get(TokenTypes.RefreshToken)
        });

        request.Content = new FormUrlEncodedContent(content);

        var tokenResult = await _httpClient.SendAsync(request, cancellationToken);
        if (!tokenResult.IsSuccessStatusCode) return responseMessage;

        var tokenResponseJson = await tokenResult.Content.ReadAsStringAsync(cancellationToken);
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(tokenResponseJson);
        if (tokenResponse is null) return responseMessage;

        var metadata = new AuthenticationMetadata
        {
            Tokens =
            [
                new AuthenticationToken { Name = TokenTypes.AccessToken, Value = tokenResponse.AccessToken },
                new AuthenticationToken { Name = TokenTypes.RefreshToken, Value = tokenResponse.RefreshToken! },
                new AuthenticationToken { Name = TokenTypes.IdToken, Value = tokenResponse.IdToken! }
            ]
        };
        
        await _tokenProvider.SetAsync(metadata, cancellationToken);

        var retry = await CloneAsync(requestMessage, cancellationToken);
        retry.Headers.WithBearerAuthentication(tokenResponse.AccessToken);

        return await _httpClient.SendAsync(retry, cancellationToken);
    }

    private async Task<HttpRequestMessage> CloneAsync(HttpRequestMessage request,
        CancellationToken cancellationToken = default)
    {
        var retryRequestMessage = new HttpRequestMessage(request.Method, request.RequestUri);

        var headers = request.Headers.Where(x => x.Key != HeaderTypes.Authorization);

        foreach (var header in headers)
            retryRequestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);

        if (request.Content != null)
        {
            var bytes = await request.Content.ReadAsByteArrayAsync(cancellationToken);
            retryRequestMessage.Content = new ByteArrayContent(bytes);

            foreach (var header in request.Content.Headers)
                retryRequestMessage.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return retryRequestMessage;
    }
}
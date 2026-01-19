using System.Net;
using System.Text.Json;
using eCinema.Server.Security.Authentication.Oidc.Constants;
using eCinema.Server.Security.Authentication.Oidc.Session;
using eCinema.Server.Security.Authentication.Oidc.Token;
using eSystem.Core.Http;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Security.Authentication.Oidc.Client;
using eSystem.Core.Security.Authentication.Oidc.Constants;
using eSystem.Core.Security.Authentication.Oidc.Token;
using eSystem.Core.Security.Cryptography.Encoding;
using Microsoft.Extensions.Options;

namespace eCinema.Server.Common.Api;

public class ApiClient(
    HttpClient httpClient,
    ISessionProvider sessionProvider,
    ITokenProvider tokenProvider,
    IOptions<ClientOptions> options) : IApiClient
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ISessionProvider _sessionProvider = sessionProvider;
    private readonly ITokenProvider _tokenProvider = tokenProvider;
    private readonly ClientOptions _clientOptions = options.Value;

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

            var serializationOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            var error = await response.Content.ReadAsync<Error>(serializationOptions, cancellationToken);
            return ApiResponse.Fail(error);
        }
        catch (Exception ex)
        {
            return ApiResponse.Fail(new Error()
            {
                Code = ErrorTypes.Common.InternalServerError,
                Description = ex.Message
            });
        }
    }

    private async Task<HttpResponseMessage> SendRequestAsync(
        ApiRequest request, ApiOptions options, CancellationToken cancellationToken)
    {
        var httpRequestMessage = await InitializeAsync(request, options, cancellationToken);
        var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage, cancellationToken);
        
        if (httpResponseMessage.StatusCode != HttpStatusCode.Unauthorized)
            return httpResponseMessage;

        var responseJson = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
        var error = JsonSerializer.Deserialize<Error>(responseJson);

        if (error is null || error.Code != ErrorTypes.OAuth.InvalidToken)
            return httpResponseMessage;

        return await RefreshAsync(httpRequestMessage, httpResponseMessage, cancellationToken);
    }

    private async Task<HttpRequestMessage> InitializeAsync(
        ApiRequest request, ApiOptions options, CancellationToken cancellationToken)
    {
        var message = new HttpRequestMessage(request.Method, request.Url);
        if (options.Authentication == AuthenticationType.Bearer)
        {
            var session = _sessionProvider.Get();
            if (session is not null)
            {
                var token = await _tokenProvider.GetAsync(
                    $"{session.Id}:{TokenTypes.AccessToken}", cancellationToken);
                
                message.Headers.WithBearerAuthentication(token);
            }
        }
        else if (options.Authentication == AuthenticationType.Basic)
        {
            message.Headers.WithBasicAuthentication(_clientOptions.ClientId, _clientOptions.ClientSecret);
        }
        
        message.AddContent(request, options);

        return message;
    }

    private async Task<HttpResponseMessage> RefreshAsync(
        HttpRequestMessage requestMessage,
        HttpResponseMessage responseMessage,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, new Uri("/api/v1/Connect/token"));
        request.Headers.WithBasicAuthentication(_clientOptions.ClientId, _clientOptions.ClientSecret);

        var session = _sessionProvider.Get();
        if (session is null) return responseMessage;

        var content = FormUrl.Encode(new TokenRequest()
        {
            ClientId = _clientOptions.ClientId,
            ClientSecret = _clientOptions.ClientSecret,
            GrantType = GrantTypes.RefreshToken,
            RefreshToken = await _tokenProvider.GetAsync(
                $"{session.Id}:{TokenTypes.RefreshToken}", cancellationToken)
        });
        
        request.Content = new FormUrlEncodedContent(content);

        var tokenResult = await _httpClient.SendAsync(request, cancellationToken);
        if (!tokenResult.IsSuccessStatusCode) return responseMessage;

        var tokenResponseJson = await tokenResult.Content.ReadAsStringAsync(cancellationToken);
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(tokenResponseJson);
        if (tokenResponse is null) return responseMessage;

        await _tokenProvider.SetAsync($"{session.Id}:{TokenTypes.AccessToken}",
            tokenResponse.AccessToken, TimeSpan.FromMinutes(5), cancellationToken);

        if (!string.IsNullOrEmpty(tokenResponse.IdToken))
        {
            await _tokenProvider.SetAsync($"{session.Id}:{TokenTypes.IdToken}",
                tokenResponse.IdToken, TimeSpan.FromMinutes(5), cancellationToken);
        }

        if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
        {
            await _tokenProvider.SetAsync($"{session.Id}:{TokenTypes.RefreshToken}",
                tokenResponse.RefreshToken, TimeSpan.FromDays(30), cancellationToken);
        }

        var retry = await CloneAsync(requestMessage, cancellationToken);
        retry.Headers.WithBearerAuthentication(tokenResponse.AccessToken);

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

        return retryRequestMessage;
    }
}
using System.Text.Json;
using eSecurity.Client.Common.Http.Extensions;
using eSecurity.Client.Common.JS.Localization;
using eSecurity.Client.Security.Authentication.Oidc.Clients;
using eSecurity.Client.Security.Authentication.Oidc.Token;
using eSystem.Core.Common.Http.Context;
using eSystem.Core.Common.Network.Gateway;
using Microsoft.Extensions.Options;

namespace eSecurity.Client.Common.Http;

public class ApiClient(
    HttpClient httpClient,
    TokenProvider tokenProvider,
    GatewayOptions gatewayOptions,
    IOptions<ClientOptions> clientOptions,
    IHttpContextAccessor httpContextAccessor) : IApiClient
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly TokenProvider _tokenProvider = tokenProvider;
    private readonly GatewayOptions _gatewayOptions = gatewayOptions;
    private readonly ClientOptions _clientOptions = clientOptions.Value;

    private readonly JsonSerializerOptions _serializationOptions = new JsonSerializerOptions()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async ValueTask<HttpResponse<TResponse>> SendAsync<TResponse>(
        HttpRequest httpRequest, HttpOptions httpOptions)
    {
        try
        {
            var httpResponseMessage = await SendRequestAsync(httpRequest, httpOptions);
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

    public async ValueTask<HttpResponse> SendAsync(HttpRequest httpRequest, HttpOptions httpOptions)
    {
        try
        {
            var httpResponseMessage = await SendRequestAsync(httpRequest, httpOptions);
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

    private async ValueTask<HttpResponseMessage> SendRequestAsync(HttpRequest httpRequest, HttpOptions httpOptions)
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
        
        message.RequestUri = new Uri($"{_gatewayOptions.Url}/{httpRequest.Url}");
        message.Method = httpRequest.Method;
        message.Headers.WithUserAgent(_httpContext.GetUserAgent());
        message.Headers.WithCookies(_httpContext.GetCookies());
        message.AddContent(httpRequest, httpOptions);
        
        return await _httpClient.SendAsync(message);
    }
}
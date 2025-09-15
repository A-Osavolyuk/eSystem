using System.Net;
using System.Text.Json;
using eShop.Blazor.Application.State;
using eShop.Blazor.Domain.Interfaces;
using eShop.Domain.Common.Http;
using eShop.Domain.Enums;
using eShop.Domain.Requests.API.Auth;
using Microsoft.AspNetCore.Http;
using AuthenticationManager = eShop.Blazor.Infrastructure.Security.AuthenticationManager;
using HttpRequest = eShop.Domain.Common.Http.HttpRequest;
using HttpResponse = eShop.Domain.Common.Http.HttpResponse;

namespace eShop.Blazor.Infrastructure.Implementations;

public class ApiClient(
    IHttpClientFactory clientFactory,
    ITokenProvider tokenProvider,
    IHttpContextAccessor httpContextAccessor,
    IConfiguration configuration,
    UserState userState,
    AuthenticationManager authenticationManager) : IApiClient
{
    private readonly IHttpClientFactory clientFactory = clientFactory;
    private readonly ITokenProvider tokenProvider = tokenProvider;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly IConfiguration configuration = configuration;
    private readonly UserState userState = userState;
    private readonly AuthenticationManager authenticationManager = authenticationManager;
    private const string GatewayKey = "services:proxy:http:0";

    public async ValueTask<HttpResponse> SendAsync(HttpRequest httpRequest, HttpOptions options)
    {
        try
        {
            var message = new HttpRequestMessage();

            if (options.WithBearer)
            {
                var token = await tokenProvider.GetAsync();

                if (string.IsNullOrEmpty(token))
                {
                    return new ResponseBuilder()
                        .Failed()
                        .WithMessage("Unauthorized")
                        .Build();
                }

                message.Headers.Add("Authorization", $"Bearer {token}");
            }
            
            var userAgent = httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString();
            message.Headers.Add("User-Agent", userAgent);

            message.RequestUri = new Uri(httpRequest.Url);
            message.Method = httpRequest.Method;

            switch (options.Type)
            {
                case DataType.Text:
                {
                    if (httpRequest.Data is not null)
                    {
                        message.Content = new StringContent(JsonSerializer.Serialize(httpRequest.Data),
                            Encoding.UTF8, "application/json");
                    }

                    break;
                }
                case DataType.File:
                {
                    message.Headers.Add("Accept", "multipart/form-data");

                    var content = new MultipartFormDataContent();

                    if (httpRequest.Data is not null)
                    {
                        if (httpRequest.Data is IReadOnlyList<IBrowserFile> files)
                        {
                            foreach (var file in files)
                            {
                                var fileContent = new StreamContent(file.OpenReadStream());
                                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                                content.Add(fileContent, "files", file.Name);
                            }
                        }

                        var metadata = JsonSerializer.Serialize(httpRequest.Metadata);
                        content.Add(new StringContent(metadata), "metadata");

                        message.Content = content;
                    }

                    break;
                }
                default: throw new NotSupportedException("Unsupported request type");
            }
            
            var serializationOptions = new JsonSerializerOptions()
            {
                WriteIndented = true, 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var httpClient = clientFactory.CreateClient("eShop.Client");
            var httpResponse = await httpClient.SendAsync(message);

            if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                var refreshResponse = await RefreshAsync();
                if (!refreshResponse.Success) await authenticationManager.UnauthorizedAsync();
                else httpResponse = await httpClient.SendAsync(message);
            }
            
            var responseJson = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<HttpResponse>(responseJson, serializationOptions)!;

            return response;
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
        var gateway = configuration[GatewayKey]!;
        var url = $"{gateway}/api/v1/Security/refresh-token";
        var refreshTokenRequest = new RefreshTokenRequest() { UserId = userState.UserId };

        var request = new HttpRequest()
        {
            Method = HttpMethod.Post,
            Url = url,
            Data = refreshTokenRequest
        };
        
        var options = new HttpOptions() { Type = DataType.Text, WithBearer = false };
        var response = await SendAsync(request, options);
        return response;
    }
}
using System.Net;
using eShop.Blazor.Server.Application.State;
using eShop.Blazor.Server.Domain.Interfaces;
using eShop.Blazor.Server.Infrastructure.Security;
using eShop.Domain.Common.Http;
using eShop.Domain.Enums;
using eShop.Domain.Requests.Auth;
using eShop.Domain.Responses.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

namespace eShop.Blazor.Server.Infrastructure.Implementations;

public class ApiClient(
    IHttpClientFactory clientFactory,
    IHttpContextAccessor httpContextAccessor,
    IConfiguration configuration,
    TokenProvider tokenProvider,
    UserState userState,
    NavigationManager navigationManager) : IApiClient
{
    private readonly IHttpClientFactory clientFactory = clientFactory;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly IConfiguration configuration = configuration;
    private readonly TokenProvider tokenProvider = tokenProvider;
    private readonly UserState userState = userState;
    private readonly NavigationManager navigationManager = navigationManager;
    private const string Key = "services:proxy:http:0";

    public async ValueTask<HttpResponse> SendAsync(HttpRequest httpRequest, HttpOptions options)
    {
        try
        {
            var message = new HttpRequestMessage();

            if (options.WithBearer)
            {
                if (string.IsNullOrEmpty(tokenProvider.AccessToken))
                {
                    var result = await RefreshTokenAsync();
                    if (!result.Success)
                    {
                        return new ResponseBuilder()
                            .Failed()
                            .WithMessage("Unauthorized")
                            .Build();
                    }

                    var response = result.Get<RefreshTokenResponse>()!;
                    tokenProvider.AccessToken = response.AccessToken;
                }

                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenProvider.AccessToken);
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
            var httpResponseMessage = await httpClient.SendAsync(message);

            if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
            {
                var result = await RefreshTokenAsync();
                
                if (!result.Success)
                {
                    tokenProvider.Clear();
                    navigationManager.NavigateTo("/account/login");
                }
                else
                {
                    var response = result.Get<RefreshTokenResponse>()!;
                    tokenProvider.AccessToken = response.AccessToken;
                    message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenProvider.AccessToken);
                    
                    httpResponseMessage = await httpClient.SendAsync(message);
                }
            }
            
            var responseJson = await httpResponseMessage.Content.ReadAsStringAsync();
            var httpResponse = JsonSerializer.Deserialize<HttpResponse>(responseJson, serializationOptions)!;

            return httpResponse;
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

    private async ValueTask<HttpResponse> RefreshTokenAsync()
    {
        var gateway = configuration[Key]!;
        var url = $"{gateway}/api/v1/Security/refresh-token";
        
        var request = new HttpRequest
        {
            Url = url,
            Method = HttpMethod.Post,
            Data = new RefreshTokenRequest() { UserId = userState.UserId }
        };

        var options = new HttpOptions
        {
            WithBearer = false,
            Type = DataType.Text
        };
        
        return await SendAsync(request, options);
    }
}
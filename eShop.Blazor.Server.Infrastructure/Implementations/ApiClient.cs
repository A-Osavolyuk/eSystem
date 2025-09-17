using System.Net;
using System.Text.Json;
using eShop.Blazor.Server.Application.Routing;
using eShop.Blazor.Server.Domain.Interfaces;
using eShop.Blazor.Server.Domain.Options;
using eShop.Blazor.Server.Infrastructure.Security;
using eShop.Domain.Common.Http;
using eShop.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace eShop.Blazor.Server.Infrastructure.Implementations;

public class ApiClient(
    IHttpClientFactory clientFactory,
    IHttpContextAccessor httpContextAccessor,
    IFetchClient fetchClient,
    TokenProvider tokenProvider,
    RouteManager routeManager) : IApiClient
{
    private readonly IHttpClientFactory clientFactory = clientFactory;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly IFetchClient fetchClient = fetchClient;
    private readonly TokenProvider tokenProvider = tokenProvider;
    private readonly RouteManager routeManager = routeManager;

    public async ValueTask<HttpResponse> SendAsync(HttpRequest httpRequest, HttpOptions options)
    {
        try
        {
            var message = new HttpRequestMessage();

            if (options.WithBearer)
            {
                var token = tokenProvider.AccessToken;

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
                
                if (!refreshResponse.Success)
                {
                    tokenProvider.Clear();
                    routeManager.Route("/account/login");
                }
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
        var fetchOptions = new FetchOptions()
        {
            Url = "/api/v1/Security/refresh-token",
            Method = HttpMethod.Post,
            Body = null,
            Credentials = Credentials.Include
        };
        
        var response = await fetchClient.FetchAsync(fetchOptions);
        return response;
    }
}
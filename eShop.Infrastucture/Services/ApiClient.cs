using System.Text.Json;
using eShop.Domain.Common.API;
using eShop.Domain.Common.Http;
using eShop.Domain.Enums;
using Microsoft.AspNetCore.Http;
using HttpRequest = eShop.Domain.Common.Http.HttpRequest;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace eShop.Infrastructure.Services;

public class ApiClient(
    IHttpClientFactory clientFactory,
    ITokenProvider tokenProvider,
    IHttpContextAccessor httpContextAccessor)
    : IApiClient
{
    private readonly HttpClient httpClient = clientFactory.CreateClient("eShop.Client");
    private readonly ITokenProvider tokenProvider = tokenProvider;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private const string Key = "services:proxy:http:0";

    public async ValueTask<Response> SendAsync(HttpRequest httpRequest, HttpOptions options)
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
                    message.Headers.Add("Accept", "application/json");

                    if (httpRequest.Data is not null)
                    {
                        message.Content = new StringContent(JsonConvert.SerializeObject(httpRequest.Data),
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

            var httpResponse = await httpClient.SendAsync(message);
            var responseJson = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<Response>(responseJson, serializationOptions)!;

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
}
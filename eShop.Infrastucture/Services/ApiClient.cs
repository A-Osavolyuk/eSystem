using eShop.Domain.Common.API;
using eShop.Domain.Enums;
using eShop.Domain.Options;
using eShop.Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using HttpRequest = eShop.Domain.Common.API.HttpRequest;

namespace eShop.Infrastructure.Services;

public class ApiClient(
    IHttpClientFactory clientFactory, 
    ITokenProvider tokenProvider,
    TokenHandler tokenHandler)
    : IApiClient
{
    private readonly HttpClient httpClient = clientFactory.CreateClient("eShop.Client");
    private readonly ITokenProvider tokenProvider = tokenProvider;
    private readonly TokenHandler tokenHandler = tokenHandler;

    public async ValueTask<Response> SendAsync(HttpRequest httpRequest, HttpOptions options)
    {
        try
        {
            var message = new HttpRequestMessage();

            if (options.WithBearer)
            {
                var token = await tokenProvider.GetTokenAsync();

                if (options.ValidateToken)
                {
                    var valid = tokenHandler.Validate(token);

                    if (valid)
                    {
                        message.Headers.Add("Authorization", $"Bearer {token}");
                    }
                    else
                    {
                        //TODO: refresh token on invalid
                    }
                }
                else
                {
                    message.Headers.Add("Authorization", $"Bearer {token}");
                }
            }
            
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
                        var files = (httpRequest.Data as IReadOnlyList<IBrowserFile>)!;
                        
                        foreach (var file in files)
                        {
                            var fileContent = new StreamContent(file.OpenReadStream());
                            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                            content.Add(fileContent, "files", file.Name);
                        }
                    }

                    var metadata = JsonConvert.SerializeObject(httpRequest.Metadata);
                    content.Add(new StringContent(metadata), "metadata");

                    message.Content = content;
                    
                    break;
                }
            }
            
            var httpResponse = await httpClient.SendAsync(message);
            var json = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Response>(json)!;
            
            return response;
        }
        catch (Exception ex)
        {
            return new ResponseBuilder()
                .Failed()
                .WithMessage(ex.Message)
                .Build();
        }
    }
}
using eShop.Domain.Common.API;
using eShop.Domain.Options;
using eShop.Infrastructure.Security;

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
            HttpRequestMessage message = new();

            message.Headers.Add("Accept", "application/json");

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

            if (httpRequest.Data is not null)
                message.Content = new StringContent(JsonConvert.SerializeObject(httpRequest.Data),
                    Encoding.UTF8, "application/json");

            message.Method = httpRequest.Method;

            var httpResponse = await httpClient.SendAsync(message);

            return await HandleStatusCode(httpResponse);
        }
        catch (Exception ex)
        {
            return new ResponseBuilder()
                .Failed()
                .WithMessage(ex.Message)
                .Build();
        }
    }

    public async ValueTask<Response> SendAsync(FileRequest request, HttpOptions options)
    {
        try
        {
            HttpRequestMessage message = new();

            message.Headers.Add("Accept", "multipart/form-data");
            message.RequestUri = new Uri(request.Url);
            message.Method = request.Method;

            if (options.WithBearer)
            {
                var token = await tokenProvider.GetTokenAsync();
                message.Headers.Add("Authorization", $"Bearer {token}");
            }

            var content = new MultipartFormDataContent();

            if (request.Data.Files.Any())
            {
                foreach (var file in request.Data.Files)
                {
                    var fileContent = new StreamContent(file.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    content.Add(fileContent, "files", file.Name);
                }
            }

            var metadata = JsonConvert.SerializeObject(request.Metadata);
            content.Add(new StringContent(metadata), "metadata");

            message.Content = content;

            var httpResponse = await httpClient.SendAsync(message);

            return await HandleStatusCode(httpResponse);
        }
        catch(Exception ex)
        {
            return new ResponseBuilder()
                .Failed()
                .WithMessage(ex.Message)
                .Build();
        }
    }

    private async ValueTask<Response> HandleStatusCode(HttpResponseMessage httpResponse)
    {
        var json = await httpResponse.Content.ReadAsStringAsync();
        var response = JsonConvert.DeserializeObject<Response>(json)!;
        return response;
    }
}
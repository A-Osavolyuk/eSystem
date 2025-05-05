using eShop.Domain.Common.API;

namespace eShop.Infrastructure.Services;

public class HttpClientService(
    IHttpClientFactory clientFactory, 
    ITokenProvider tokenProvider,
    JwtAuthenticationHandler jwtAuthenticationHandler)
    : IHttpClientService
{
    private readonly HttpClient httpClient = clientFactory.CreateClient("eShop.Client");
    private readonly ITokenProvider tokenProvider = tokenProvider;
    private readonly JwtAuthenticationHandler jwtAuthenticationHandler = jwtAuthenticationHandler;

    public async ValueTask<Response> SendAsync(Request request, bool withBearer = true)
    {
        try
        {
            HttpRequestMessage message = new();

            message.Headers.Add("Accept", "application/json");

            if (withBearer)
            {
                var token = await tokenProvider.GetTokenAsync();
                message.Headers.Add("Authorization", $"Bearer {token}");
            }

            message.RequestUri = new Uri(request.Url);

            if (request.Data is not null)
                message.Content = new StringContent(JsonConvert.SerializeObject(request.Data),
                    Encoding.UTF8, "application/json");

            message.Method = request.Method;

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

    public async ValueTask<Response> SendAsync(FileRequest request, bool withBearer = true)
    {
        try
        {
            HttpRequestMessage message = new();

            message.Headers.Add("Accept", "multipart/form-data");
            message.RequestUri = new Uri(request.Url);
            message.Method = request.Method;

            if (withBearer)
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
        var response = JsonConvert.DeserializeObject<Response>(json);

        return httpResponse.StatusCode switch
        {
            HttpStatusCode.OK => response!,
            _ => new ResponseBuilder().Failed().WithMessage(response!.Message).Build(),
        };
    }
}
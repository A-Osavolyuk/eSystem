using eShop.Domain.Common.API;
using eShop.Domain.Enums;
using eShop.Domain.Options;
using eShop.Infrastructure.Security;
using HttpRequest = eShop.Domain.Common.API.HttpRequest;

namespace eShop.Infrastructure.Services;

public class ApiClient(
    IHttpClientFactory clientFactory,
    ITokenProvider tokenProvider,
    TokenHandler tokenHandler,
    AuthenticationManager authenticationManager)
    : IApiClient
{
    private readonly HttpClient httpClient = clientFactory.CreateClient("eShop.Client");
    private readonly ITokenProvider tokenProvider = tokenProvider;
    private readonly TokenHandler tokenHandler = tokenHandler;
    private readonly AuthenticationManager authenticationManager = authenticationManager;

    public async ValueTask<Response> SendAsync(HttpRequest httpRequest, HttpOptions options)
    {
        try
        {
            var message = new HttpRequestMessage();

            if (options.WithBearer)
            {
                var token = await tokenProvider.GetTokenAsync();

                if (string.IsNullOrEmpty(token))
                {
                    await authenticationManager.LogOutAsync();
                    return new ResponseBuilder()
                        .Failed()
                        .WithMessage("Unauthorized")
                        .Build();
                }

                var valid = tokenHandler.Validate(token);

                if (valid)
                {
                    message.Headers.Add("Authorization", $"Bearer {token}");
                }
                else
                {
                    //TODO: refresh token
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
                        if (httpRequest.Data is IReadOnlyList<IBrowserFile> files)
                        {
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
                    }

                    break;
                }
                default: throw new NotSupportedException("Unsupported request type");
            }

            var httpResponse = await httpClient.SendAsync(message);
            var responseJson = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Response>(responseJson)!;

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
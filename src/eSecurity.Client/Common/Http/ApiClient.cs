using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using eSecurity.Client.Common.JS.Localization;
using eSystem.Core.Primitives;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace eSecurity.Client.Common.Http;

public class ApiClient(
    HttpClient httpClient,
    ILocalizationManager localizationManager) : IApiClient
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILocalizationManager _localizationManager = localizationManager;

    public async ValueTask<ApiResponse> SendAsync(ApiRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var httpMethod = HttpHelper.Map(request.Method);
            var message = new HttpRequestMessage(httpMethod, request.Url);
            if (request.Data is not null)
            {
                message.Content = new StringContent(JsonSerializer.Serialize(request.Data),
                    Encoding.UTF8, ContentTypes.Application.Json);
            }
            message.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            
            var response = await _httpClient.SendAsync(message, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                return ApiResponse.Success(content);
            }

            var serializationOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var error = await response.Content.ReadFromJsonAsync<Error>(serializationOptions, cancellationToken);
            if (error is not null)
                return ApiResponse.Fail(error);
            
            return ApiResponse.Fail(new Error
            {
                Code = ErrorCode.InternalServerError,
                Description = "Invalid response"
            });
        }
        catch (Exception ex)
        {
            return ApiResponse.Fail(new Error
            {
                Code = ErrorCode.InternalServerError,
                Description = ex.Message
            });
        }
    }
}
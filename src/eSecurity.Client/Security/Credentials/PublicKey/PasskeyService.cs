using eSecurity.Client.Common.Http;
using eSecurity.Core.Requests;

namespace eSecurity.Client.Security.Credentials.PublicKey;

public class PasskeyService(IApiClient apiClient) : IPasskeyService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<ApiResponse> GenerateRequestOptionsAsync(
        GenerateRequestOptionsRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Passkey/options/request"
            });

    public async ValueTask<ApiResponse> GenerateCreationOptionsAsync(
        GenerateCreationOptionsRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Passkey/options/creation"
            });

    public async ValueTask<ApiResponse> CreateAsync(CreatePasskeyRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Passkey/create"
            });

    public async ValueTask<ApiResponse> ChangeNameAsync(ChangePasskeyNameRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Passkey/change-name"
            });

    public async ValueTask<ApiResponse> RemoveAsync(RemovePasskeyRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Passkey/remove"
            });
}
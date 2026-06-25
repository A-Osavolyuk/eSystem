using eSecurity.Client.Common.Http;
using eSecurity.Core.Requests;

namespace eSecurity.Client.Security.Credentials.PublicKey;

public class SoftwareService(IApiClient apiClient) : ISoftwareService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<ApiResponse> GenerateRequestOptionsAsync(
        GenerateRequestOptionsRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/software-key/options/request"
            });

    public async ValueTask<ApiResponse> GenerateCreationOptionsAsync(
        GenerateCreationOptionsRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/software-key/options/creation"
            });

    public async ValueTask<ApiResponse> CreateAsync(CreateSoftwareKeyRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/software-key/create"
            });

    public async ValueTask<ApiResponse> ChangeNameAsync(ChangeSoftwareKeyNameRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/software-key/change-name"
            });

    public async ValueTask<ApiResponse> RemoveAsync(RemoveSoftwareKeyRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/software-key/remove"
            });
}
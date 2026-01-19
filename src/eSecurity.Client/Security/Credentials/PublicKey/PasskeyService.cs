using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Credentials.PublicKey;

public class PasskeyService(IApiClient apiClient) : IPasskeyService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<ApiResponse> GenerateRequestOptionsAsync(
        GenerateRequestOptionsRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "/api/v1/Passkey/options/request"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<ApiResponse> GenerateCreationOptionsAsync(
        GenerateCreationOptionsRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "/api/v1/Passkey/options/creation"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> CreateAsync(CreatePasskeyRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "/api/v1/Passkey/create"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> ChangeNameAsync(ChangePasskeyNameRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "/api/v1/Passkey/change-name"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> RemoveAsync(RemovePasskeyRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "/api/v1/Passkey/remove"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });
}
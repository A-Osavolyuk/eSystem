using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Identity;

public class UsernameService(IApiClient apiClient) : IUsernameService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<ApiResponse> SetUsernameAsync(SetUsernameRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Data = request,
                Method = HttpMethod.Post,
                Url = "/api/v1/Username/set"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<ApiResponse> ChangeUsernameAsync(ChangeUsernameRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Data = request,
                Method = HttpMethod.Put,
                Url = "/api/v1/Username/change"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> CheckUsernameAsync(CheckUsernameRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Data = request,
                Method = HttpMethod.Post,
                Url = "/api/v1/Username/check"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });
}
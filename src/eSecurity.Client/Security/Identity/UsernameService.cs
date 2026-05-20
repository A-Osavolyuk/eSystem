using eSecurity.Client.Common.Http;
using eSecurity.Core.Requests;

namespace eSecurity.Client.Security.Identity;

public class UsernameService(IApiClient apiClient) : IUsernameService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<ApiResponse> SetUsernameAsync(SetUsernameRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Data = request,
                Method = HttpMethods.Post,
                Url = "/api/v1/Username/set"
            });

    public async ValueTask<ApiResponse> CheckUsernameAsync(CheckUsernameRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Data = request,
                Method = HttpMethods.Post,
                Url = "/api/v1/Username/check"
            });
}
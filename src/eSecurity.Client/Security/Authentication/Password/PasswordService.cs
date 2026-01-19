using eSecurity.Client.Common.Http;
using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Authentication.Password;

public class PasswordService(IApiClient apiClient) : IPasswordService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<ApiResponse> AddPasswordAsync(AddPasswordRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Password/add"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Password/forgot"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<ApiResponse> ResetPasswordAsync(ResetPasswordRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Password/reset"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<ApiResponse> ChangePasswordAsync(ChangePasswordRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Password/change"
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });
}
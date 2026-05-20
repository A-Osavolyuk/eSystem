using eSecurity.Client.Common.Http;
using eSecurity.Core.Requests;

namespace eSecurity.Client.Security.Authentication.Password;

public class PasswordService(IApiClient apiClient) : IPasswordService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<ApiResponse> AddPasswordAsync(AddPasswordRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Password/add"
            });

    public async ValueTask<ApiResponse> RequestForgotPasswordAsync(ForgotPasswordRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Password/forgot/request"
            });

    public async ValueTask<ApiResponse> ConfirmForgotPasswordAsync(ConfirmForgotPasswordRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Password/forgot/confirm"
            });

    public async ValueTask<ApiResponse> ResetPasswordAsync(ResetPasswordRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Password/reset"
            });

    public async ValueTask<ApiResponse> SetPasswordAsync(SetPasswordRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Password/set"
            });

    public async ValueTask<ApiResponse> ChangePasswordAsync(ChangePasswordRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Password/change"
            });
}
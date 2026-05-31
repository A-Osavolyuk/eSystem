using eSecurity.Client.Common.Http;
using eSecurity.Core.Requests;
using eSecurity.Core.Requests.Email;

namespace eSecurity.Client.Security.Identity;

public class EmailService(IApiClient apiClient) : IEmailService
{
    private readonly IApiClient _apiClient = apiClient;
    
        public async ValueTask<ApiResponse> AddEmailAsync(AddEmailRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Email/add"
            });

    public async ValueTask<ApiResponse> CheckEmailAsync(CheckEmailRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Email/check"
            });

    public async ValueTask<ApiResponse> ChangeEmailAsync(ChangeEmailRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Email/change"
            });

    public async ValueTask<ApiResponse> ManageEmailAsync(ManageEmailRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Email/manage"
            });

    public async ValueTask<ApiResponse> RemoveEmailAsync(RemoveEmailRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Email/remove"
            });

    public async ValueTask<ApiResponse> ResetEmailAsync(ResetEmailRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Email/reset"
            });

    public async ValueTask<ApiResponse> ConfirmEmailVerificationAsync(ConfirmEmailVerificationRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Email/verification/confirm"
            });

    public async ValueTask<ApiResponse> SendEmailVerificationAsync(SendEmailVerificationRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Email/verification/send"
            });
    public async ValueTask<ApiResponse> ResendEmailVerificationAsync(ResendEmailVerificationRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Email/verification/resend"
            });
}
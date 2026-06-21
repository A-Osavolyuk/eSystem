using eSecurity.Client.Common.Http;
using eSecurity.Core.Requests;
using eSecurity.Core.Requests.Email.Change;
using eSecurity.Core.Requests.Email.Reset;
using eSecurity.Core.Requests.Email.Verification;

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

    public async ValueTask<ApiResponse> RemoveEmailAsync(RemoveEmailRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Email/remove"
            });

    public async ValueTask<ApiResponse> VerifyEmailAsync(VerifyEmailRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Email/verification/confirm"
            });

    public async ValueTask<ApiResponse> SendEmailVerificationAsync(SendEmailVerificationOtpRequest otpRequest)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = otpRequest,
                Url = "/api/v1/Email/verification/send-otp"
            });

    public async ValueTask<ApiResponse> SendEmailChangeAsync(SendEmailChangeOtpRequest otpRequest)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = otpRequest,
                Url = "/api/v1/Email/change/send-otp"
            });

    public async ValueTask<ApiResponse> ChangeEmailAsync(ChangeEmailRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Email/change/confirm"
            });

    public async ValueTask<ApiResponse> SendEmailResetAsync(SendEmailResetOtpRequest otpRequest)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = otpRequest,
                Url = "/api/v1/Email/reset/send-otp"
            });

    public async ValueTask<ApiResponse> ResetEmailAsync(ResetEmailRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Email/reset/confirm"
            });
}
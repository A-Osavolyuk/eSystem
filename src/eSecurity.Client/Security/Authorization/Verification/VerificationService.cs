using eSecurity.Client.Common.Http;
using eSecurity.Core.Requests;
using eSecurity.Core.Requests.Verification;
using VerificationRequest = eSecurity.Core.Requests.VerificationRequest;

namespace eSecurity.Client.Security.Authorization.Verification;

public class VerificationService(IApiClient apiClient) : IVerificationService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<ApiResponse> SendCodeAsync(SendCodeRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Verification/code/send"
            });

    public async ValueTask<ApiResponse> ResendCodeAsync(ResendCodeRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Verification/code/resend"
            });

    public async ValueTask<ApiResponse> VerifyAsync(VerificationRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Verification/request-verification"
            });

    public async ValueTask<ApiResponse> SendEmailOtpAsync()
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Url = "/api/v1/Verification/email-otp/send"
            });

    public async ValueTask<ApiResponse> ResendEmailOtpAsync()
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Url = "/api/v1/Verification/email-otp/resend"
            });

    public async ValueTask<ApiResponse> VerifyEmailOtpAsync(VerifyEmailOtpRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Verification/email-otp/verify"
            });

    public async ValueTask<ApiResponse> VerifyAuthenticatorAppAsync(VerifyAuthenticatorAppRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Verification/authenticator-app/verify"
            });

    public async ValueTask<ApiResponse> VerifySoftwareKeyAsync(VerifySoftwareKeyRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Data = request,
                Url = "/api/v1/Verification/software-key/verify"
            });
}
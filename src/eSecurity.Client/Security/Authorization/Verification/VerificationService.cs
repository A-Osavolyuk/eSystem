using eSecurity.Client.Common.Http;
using eSecurity.Core.Requests.Verification;

namespace eSecurity.Client.Security.Authorization.Verification;

public class VerificationService(IApiClient apiClient) : IVerificationService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<ApiResponse> SendEmailOtpAsync(SendEmailOtpRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Url = "/api/v1/Verification/email-otp/send",
                Data = request
            });

    public async ValueTask<ApiResponse> ResendEmailOtpAsync(ResendEmailOtpRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Url = "/api/v1/Verification/email-otp/resend",
                Data = request
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
using eSecurity.Client.Common.Http;
using eSecurity.Core.Requests;

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
                Url = "/api/v1/Verification/passkey/request-verification"
            });
}
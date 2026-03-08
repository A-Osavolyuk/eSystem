using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Authorization.Access.Verification;

public class VerificationService(IApiClient apiClient) : IVerificationService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<ApiResponse> SendCodeAsync(SendCodeRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "/api/v1/Verification/code/send"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> ResendCodeAsync(ResendCodeRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "/api/v1/Verification/code/resend"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<ApiResponse> VerifyAsync(VerificationRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "/api/v1/Verification/passkey/request-verification"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });
}
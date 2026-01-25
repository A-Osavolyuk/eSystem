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
                Authentication = AuthenticationType.None
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
                Authentication = AuthenticationType.None
            });


    public async ValueTask<ApiResponse> VerifyCodeAsync(VerifyCodeRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "/api/v1/Verification/code/verify"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<ApiResponse> VerifyRecoveryCodeAsync(VerifyRecoveryCodeRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "/api/v1/Verification/recovery-code/verify"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });


    public async ValueTask<ApiResponse> VerifyAuthenticatorCodeAsync(VerifyAuthenticatorCodeRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "/api/v1/Verification/authenticator/verify"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });


    public async ValueTask<ApiResponse> VerifyPasskeyAsync(VerifyPasskeyRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "/api/v1/Verification/passkey/verify"
            }, new ApiOptions
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });
}
using eSecurity.Client.Common.Http;
using eSecurity.Core.Common.Requests;
using eSystem.Core.Common.Http;

namespace eSecurity.Client.Security.Authorization.Access.Verification;

public class VerificationService(IApiClient apiClient) : IVerificationService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<Result> SendCodeAsync(SendCodeRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Verification/code/send"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<Result> ResendCodeAsync(ResendCodeRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Verification/code/resend"
            }, new HttpOptions() { Type = DataType.Text });


    public async ValueTask<Result> VerifyCodeAsync(VerifyCodeRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Verification/code/verify"
            }, new HttpOptions() { Type = DataType.Text });


    public async ValueTask<Result> VerifyAuthenticatorCodeAsync(VerifyAuthenticatorCodeRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Verification/authenticator/verify"
            }, new HttpOptions() { Type = DataType.Text });


    public async ValueTask<Result> VerifyPasskeyAsync(VerifyPasskeyRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Verification/passkey/verify"
            }, new HttpOptions() { Type = DataType.Text });

}
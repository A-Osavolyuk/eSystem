using eAccount.Common.Http;
using eSystem.Core.Common.Http;
using eSystem.Core.Requests.Auth;

namespace eAccount.Security.Authorization.Access;

public class VerificationService(
    IApiClient apiClient) : IVerificationService
{
    private readonly IApiClient apiClient = apiClient;
    private const string BasePath = "api/v1/Verification";
    
    public async ValueTask<HttpResponse> SendCodeAsync(SendCodeRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/code/send", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> ResendCodeAsync(ResendCodeRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/code/resend", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> VerifyCodeAsync(VerifyCodeRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/code/verify", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> VerifyAuthenticatorCodeAsync(VerifyAuthenticatorCodeRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/authenticator/verify", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> VerifyPasskeyAsync(VerifyPasskeyRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/passkey/verify", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });
}
using eAccount.Blazor.Server.Domain.Abstraction.Services;

namespace eAccount.Blazor.Server.Infrastructure.Implementations;

public class VerificationService(
    IConfiguration configuration, 
    IApiClient apiClient) : ApiService(configuration, apiClient), IVerificationService
{
    private const string BasePath = "api/v1/Verification";
    
    public async ValueTask<HttpResponse> SendCodeAsync(SendCodeRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/code/send", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = false, Type = DataType.Text });

    public async ValueTask<HttpResponse> ResendCodeAsync(ResendCodeRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/code/resend", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = false, Type = DataType.Text });

    public async ValueTask<HttpResponse> VerifyCodeAsync(VerifyCodeRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/code/verify", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = false, Type = DataType.Text });

    public async ValueTask<HttpResponse> VerifyAuthenticatorCodeAsync(VerifyAuthenticatorCodeRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/authenticator/verify", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = false, Type = DataType.Text });

    public async ValueTask<HttpResponse> VerifyPasskeyAsync(VerifyPasskeyRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/passkey/verify", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = false, Type = DataType.Text });
}
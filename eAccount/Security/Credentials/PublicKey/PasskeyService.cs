using eAccount.Common.Http;
using eSystem.Core.Common.Http;
using eSystem.Core.Requests.Auth;

namespace eAccount.Security.Credentials.PublicKey;

public class PasskeyService(IApiClient apiClient) : IPasskeyService
{
    private readonly IApiClient apiClient = apiClient;
    private const string BasePath = "api/v1/Passkey";
    
    public async ValueTask<HttpResponse> ChangeNameAsync(ChangePasskeyNameRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/change-name", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> GenerateCreationOptionsAsync(GenerateCreationOptionsRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/options/attestation", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text, WithBearer = true});
    
    public async ValueTask<HttpResponse> GenerateRequestOptionsAsync(
        GenerateRequestOptionsRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/options/assertion", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> CreatAsync(CreatePasskeyRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/create", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> RemoveAsync(RemovePasskeyRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{BasePath}/remove", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text, WithBearer = true });
}
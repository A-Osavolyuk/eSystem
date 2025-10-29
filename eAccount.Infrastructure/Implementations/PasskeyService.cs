using eAccount.Domain.Abstraction.Services;
using eSystem.Core.Common.Network.Gateway;
using eSystem.Core.Requests.Auth;

namespace eAccount.Infrastructure.Implementations;

public class PasskeyService(
    GatewayOptions gateway, 
    IApiClient apiClient) : IPasskeyService
{
    private readonly GatewayOptions gateway = gateway;
    private readonly IApiClient apiClient = apiClient;
    private const string BasePath = "api/v1/Passkey";
    
    public async ValueTask<HttpResponse> GetAsync(Guid id) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{gateway.Url}/{BasePath}/{id}", Method = HttpMethod.Get },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> ChangeNameAsync(ChangePasskeyNameRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{gateway.Url}/{BasePath}/change-name", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> GenerateCreationOptionsAsync(GenerateCreationOptionsRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{gateway.Url}/{BasePath}/options/attestation", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });
    
    public async ValueTask<HttpResponse> GenerateRequestOptionsAsync(
        GenerateRequestOptionsRequest requestOptionsRequest) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{gateway.Url}/{BasePath}/options/assertion", Method = HttpMethod.Post, Data = requestOptionsRequest },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> CreatAsync(CreatePasskeyRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{gateway.Url}/{BasePath}/create", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> RemoveAsync(RemovePasskeyRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{gateway.Url}/{BasePath}/remove", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });
}
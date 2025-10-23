using eAccount.Blazor.Server.Domain.Abstraction.Services;
using eAccount.Blazor.Server.Domain.Interfaces;
using eShop.Domain.Common.Http;
using eShop.Domain.Enums;
using eShop.Domain.Requests.Auth;

namespace eAccount.Blazor.Server.Infrastructure.Implementations;

public class PasskeyService(
    IConfiguration configuration, 
    IApiClient apiClient) : ApiService(configuration, apiClient), IPasskeyService
{
    private const string BasePath = "api/v1/Passkey";
    
    public async ValueTask<HttpResponse> GetAsync(Guid id) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/{id}", Method = HttpMethod.Get },
        new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<HttpResponse> ChangeNameAsync(ChangePasskeyNameRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/change-name", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<HttpResponse> GenerateCreationOptionsAsync(GenerateCreationOptionsRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/options/attestation", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = true, Type = DataType.Text });
    
    public async ValueTask<HttpResponse> GenerateRequestOptionsAsync(
        GenerateRequestOptionsRequest requestOptionsRequest) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/options/assertion", Method = HttpMethod.Post, Data = requestOptionsRequest },
        new HttpOptions { WithBearer = false, Type = DataType.Text });

    public async ValueTask<HttpResponse> CreatAsync(CreatePasskeyRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/create", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<HttpResponse> RemoveAsync(RemovePasskeyRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/{BasePath}/remove", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = true, Type = DataType.Text });
}
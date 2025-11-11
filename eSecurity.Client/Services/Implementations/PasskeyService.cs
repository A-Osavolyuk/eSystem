using eSecurity.Client.Common.Http;
using eSecurity.Client.Services.Interfaces;
using eSecurity.Core.Common.Requests;
using eSystem.Core.Common.Http;

namespace eSecurity.Client.Services.Implementations;

public class PasskeyService(IApiClient apiClient) : IPasskeyService
{
    private readonly IApiClient apiClient = apiClient;
    
    public async ValueTask<HttpResponse> GenerateRequestOptionsAsync(GenerateRequestOptionsRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Passkey/options/request"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<HttpResponse> GenerateCreationOptionsAsync(GenerateCreationOptionsRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Passkey/options/creation"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> CreateAsync(CreatePasskeyRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Passkey/create"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> ChangeNameAsync(ChangePasskeyNameRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Passkey/change-name"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<HttpResponse> RemoveAsync(RemovePasskeyRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Passkey/remove"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });
}
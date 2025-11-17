using eSecurity.Client.Common.Http;
using eSecurity.Core.Common.Requests;
using eSystem.Core.Common.Http;

namespace eSecurity.Client.Security.Credentials.PublicKey;

public class PasskeyService(IApiClient apiClient) : IPasskeyService
{
    private readonly IApiClient _apiClient = apiClient;
    
    public async ValueTask<Result> GenerateRequestOptionsAsync(GenerateRequestOptionsRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Passkey/options/request"
            }, new HttpOptions() { Type = DataType.Text });

    public async ValueTask<Result> GenerateCreationOptionsAsync(GenerateCreationOptionsRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Passkey/options/creation"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<Result> CreateAsync(CreatePasskeyRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Passkey/create"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<Result> ChangeNameAsync(ChangePasskeyNameRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Passkey/change-name"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });

    public async ValueTask<Result> RemoveAsync(RemovePasskeyRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Passkey/remove"
            }, new HttpOptions() { Type = DataType.Text, WithBearer = true });
}
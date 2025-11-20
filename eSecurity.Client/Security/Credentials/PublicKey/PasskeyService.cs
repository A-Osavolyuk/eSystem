using eSecurity.Client.Common.Http;
using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Credentials.PublicKey;

namespace eSecurity.Client.Security.Credentials.PublicKey;

public class PasskeyService(IApiClient apiClient) : IPasskeyService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<HttpResponse<PublicKeyCredentialRequestOptions>> GenerateRequestOptionsAsync(
        GenerateRequestOptionsRequest request)
        => await _apiClient.SendAsync<PublicKeyCredentialRequestOptions>(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Passkey/options/request"
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<HttpResponse<PublicKeyCredentialCreationOptions>> GenerateCreationOptionsAsync(
        GenerateCreationOptionsRequest request)
        => await _apiClient.SendAsync<PublicKeyCredentialCreationOptions>(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Passkey/options/creation"
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<HttpResponse> CreateAsync(CreatePasskeyRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Passkey/create"
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<HttpResponse> ChangeNameAsync(ChangePasskeyNameRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Passkey/change-name"
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });

    public async ValueTask<HttpResponse> RemoveAsync(RemovePasskeyRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Passkey/remove"
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.Bearer
            });
}
using eAccount.Domain.Abstraction.Services;
using eSystem.Core.Common.Network.Gateway;
using eSystem.Core.Requests.Storage;

namespace eAccount.Infrastructure.Implementations;

public class StorageService(
    GatewayOptions gatewayOptions,
    IApiClient apiClient) : IStoreService
{
    private readonly GatewayOptions gatewayOptions = gatewayOptions;
    private readonly IApiClient apiClient = apiClient;
    private const string BasePath = "api/v1/Files";
    
    public async ValueTask<HttpResponse> UploadFilesAsync(UploadFilesRequest request) =>
        await apiClient.SendAsync(
            new HttpRequest()
            {
                Data = request.Files,
                Method = HttpMethod.Post,
                Url = $"{gatewayOptions.Url}/{BasePath}/upload",
                Metadata = new Metadata()
                {
                    Identifier = request.Identifier,
                    Type = request.Type
                }
            },
            new HttpOptions { Type = DataType.File });
    
    public async ValueTask<HttpResponse> LoadFilesAsync(LoadFilesRequest request) =>
        await apiClient.SendAsync(
            new HttpRequest() { Data = request, Method = HttpMethod.Post, Url = $"{gatewayOptions.Url}/{BasePath}/load" },
            new HttpOptions { Type = DataType.Text });
}
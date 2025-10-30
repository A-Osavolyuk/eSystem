using eAccount.Common.Http;
using eSystem.Core.Common.Http;
using eSystem.Core.Requests.Storage;
using Metadata = eSystem.Core.Common.Http.Metadata;

namespace eAccount.Common.Storage.External;

public class StorageService(IApiClient apiClient) : IStorageService
{
    private readonly IApiClient apiClient = apiClient;
    private const string BasePath = "api/v1/Files";
    
    public async ValueTask<HttpResponse> UploadFilesAsync(UploadFilesRequest request) =>
        await apiClient.SendAsync(
            new HttpRequest()
            {
                Data = request.Files,
                Method = HttpMethod.Post,
                Url = $"{BasePath}/upload",
                Metadata = new Metadata()
                {
                    Identifier = request.Identifier,
                    Type = request.Type
                }
            },
            new HttpOptions { Type = DataType.File });
    
    public async ValueTask<HttpResponse> LoadFilesAsync(LoadFilesRequest request) =>
        await apiClient.SendAsync(
            new HttpRequest() { Data = request, Method = HttpMethod.Post, Url = $"{BasePath}/load" },
            new HttpOptions { Type = DataType.Text });
}
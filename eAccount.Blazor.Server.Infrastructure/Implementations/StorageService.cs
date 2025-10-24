using eAccount.Blazor.Server.Domain.Abstraction.Services;
using eSystem.Domain.Requests.Storage;

namespace eAccount.Blazor.Server.Infrastructure.Implementations;

class StorageService(
    IConfiguration configuration,
    IApiClient pipe) : ApiService(configuration, pipe), IStoreService
{
    private const string BasePath = "api/v1/Files";
    
    public async ValueTask<HttpResponse> UploadFilesAsync(UploadFilesRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest()
            {
                Data = request.Files,
                Method = HttpMethod.Post,
                Url = $"{Gateway}/{BasePath}/upload",
                Metadata = new Metadata()
                {
                    Identifier = request.Identifier,
                    Type = request.Type
                }
            },
            new HttpOptions { Type = DataType.File });
    
    public async ValueTask<HttpResponse> LoadFilesAsync(LoadFilesRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest() { Data = request, Method = HttpMethod.Post, Url = $"{Gateway}/{BasePath}/load" },
            new HttpOptions { Type = DataType.Text });
}
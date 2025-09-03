using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.Http;
using eShop.Domain.Enums;
using eShop.Domain.Requests.API.Storage;

namespace eShop.Infrastructure.Implementations;

class StorageService(
    IConfiguration configuration,
    IApiClient pipe) : ApiService(configuration, pipe), IStoreService
{
    public async ValueTask<HttpResponse> UploadFilesAsync(UploadFilesRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest()
            {
                Data = request.Files,
                Method = HttpMethod.Post,
                Url = $"{Gateway}/api/v1/Files/upload",
                Metadata = new Metadata()
                {
                    Identifier = request.Identifier,
                    Type = request.Type
                }
            },
            new HttpOptions { WithBearer = true, Type = DataType.File });
    
    public async ValueTask<HttpResponse> LoadFilesAsync(LoadFilesRequest request) =>
        await ApiClient.SendAsync(
            new HttpRequest() { Data = request, Method = HttpMethod.Post, Url = $"{Gateway}/api/v1/Files/load" },
            new HttpOptions { WithBearer = true, Type = DataType.Text });
}
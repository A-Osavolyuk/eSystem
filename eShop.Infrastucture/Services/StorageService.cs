using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Options;
using eShop.Domain.Requests.API.Storage;

namespace eShop.Infrastructure.Services;

class StorageService(
    IConfiguration configuration,
    IApiClient pipe) : ApiService(configuration, pipe), IStoreService
{
    public async ValueTask<Response> UploadFilesAsync(UploadFilesRequest request) =>
        await ApiClient.SendAsync(
            new FileRequest
            {
                Data = new FileData(request.Files), 
                Method = HttpMethod.Post, 
                Url = $"{Gateway}/api/v1/Files/upload",
                Metadata = new Metadata()
                {
                    Identifier = request.Identifier,
                    Type = request.Type
                }
            }, 
            new HttpOptions { ValidateToken = true, WithBearer = true });

}
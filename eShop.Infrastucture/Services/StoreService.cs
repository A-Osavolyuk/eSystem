using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Options;

namespace eShop.Infrastructure.Services;

class StoreService(
    IConfiguration configuration,
    IApiClient pipe) : ApiService(configuration, pipe), IStoreService
{
    public async ValueTask<Response> GetUserAvatarAsync(string userId) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Files/get-user-avatar/{userId}", Method = HttpMethod.Get }, 
            new HttpOptions { ValidateToken = true, WithBearer = true });

    public async ValueTask<Response> UploadProductImagesAsync(IReadOnlyList<IBrowserFile> files, Guid productId) =>
        await ApiClient.SendAsync(
            new FileRequest { Data = new FileData(files), Method = HttpMethod.Post, Url = $"{Gateway}/api/v1/Files/upload-product-images/{productId}" }, 
            new HttpOptions { ValidateToken = true, WithBearer = true });

    public async ValueTask<Response> RemoveUserAvatarAsync(string userId) =>
        await ApiClient.SendAsync(
            new HttpRequest { Url = $"{Gateway}/api/v1/Files/remove-user-avatar/{userId}", Method = HttpMethod.Delete }, 
            new HttpOptions { ValidateToken = true, WithBearer = true });

    public async ValueTask<Response> UploadUserAvatarAsync(string userId, IBrowserFile file) =>
        await ApiClient.SendAsync(
            new FileRequest { Data = new FileData([file]), Method = HttpMethod.Post, Url = $"{Gateway}/api/v1/Files/upload-user-avatar/{userId}" }, 
            new HttpOptions { ValidateToken = true, WithBearer = true });

}
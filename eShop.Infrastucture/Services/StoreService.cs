using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;

namespace eShop.Infrastructure.Services;

class StoreService(
    IConfiguration configuration, 
    IHttpClientService clientService) : ApiService(configuration, clientService), IStoreService
{
    public async ValueTask<Response> GetUserAvatarAsync(string userId) =>
        await HttpClientService.SendAsync(
            new Request(
                $"{Configuration[Key]}/api/v1/Files/get-user-avatar/{userId}",
                HttpMethod.Get));

    public async ValueTask<Response>
        UploadProductImagesAsync(IReadOnlyList<IBrowserFile> files, Guid productId) =>
        await HttpClientService.SendAsync(
            new FileRequest(new FileData(files), HttpMethod.Post,
                $"{Configuration[Key]}/api/v1/Files/upload-product-images/{productId}"));

    public async ValueTask<Response> RemoveUserAvatarAsync(string userId) =>
        await HttpClientService.SendAsync(
            new Request(
                $"{Configuration[Key]}/api/v1/Files/remove-user-avatar/{userId}",
                HttpMethod.Delete));

    public async ValueTask<Response> UploadUserAvatarAsync(string userId, IBrowserFile file) =>
        await HttpClientService.SendAsync(
            new FileRequest(new FileData([file]), HttpMethod.Post,
                $"{Configuration[Key]}/api/v1/Files/upload-user-avatar/{userId}"));
}
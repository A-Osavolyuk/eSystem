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
                HttpMethods.Get));

    public async ValueTask<Response>
        UploadProductImagesAsync(IReadOnlyList<IBrowserFile> files, Guid productId) =>
        await HttpClientService.SendFilesAsync(
            new FileRequest(new FileData(files), HttpMethods.Post,
                $"{Configuration[Key]}/api/v1/Files/upload-product-images/{productId}"));

    public async ValueTask<Response> RemoveUserAvatarAsync(string userId) =>
        await HttpClientService.SendAsync(
            new Request(
                $"{Configuration[Key]}/api/v1/Files/remove-user-avatar/{userId}",
                HttpMethods.Delete));

    public async ValueTask<Response> UploadUserAvatarAsync(string userId, IBrowserFile file) =>
        await HttpClientService.SendFilesAsync(
            new FileRequest(new FileData(file), HttpMethods.Post,
                $"{Configuration[Key]}/api/v1/Files/upload-user-avatar/{userId}"));
}
using eShop.Domain.Common.Api;

namespace eShop.Infrastructure.Services;

class StoreService(IConfiguration configuration, IHttpClientService clientService) : IStoreService
{
    private readonly IConfiguration configuration = configuration;
    private readonly IHttpClientService clientService = clientService;

    public async ValueTask<Response> GetUserAvatarAsync(string userId) =>
        await clientService.SendAsync(
            new Request(
                $"{configuration["Configuration:Services:Proxy:Gateway:Uri"]}/api/v1/Files/get-user-avatar/{userId}",
                HttpMethods.Get));

    public async ValueTask<Response>
        UploadProductImagesAsync(IReadOnlyList<IBrowserFile> files, Guid productId) =>
        await clientService.SendFilesAsync(
            new FileRequest(new FileData(files), HttpMethods.Post,
                $"{configuration["Configuration:Services:Proxy:Gateway:Uri"]}/api/v1/Files/upload-product-images/{productId}"));

    public async ValueTask<Response> RemoveUserAvatarAsync(string userId) =>
        await clientService.SendAsync(
            new Request(
                $"{configuration["Configuration:Services:Proxy:Gateway:Uri"]}/api/v1/Files/remove-user-avatar/{userId}",
                HttpMethods.Delete));

    public async ValueTask<Response> UploadUserAvatarAsync(string userId, IBrowserFile file) =>
        await clientService.SendFilesAsync(
            new FileRequest(new FileData(file), HttpMethods.Post,
                $"{configuration["Configuration:Services:Proxy:Gateway:Uri"]}/api/v1/Files/upload-user-avatar/{userId}"));

    private BlobContainerClient GetContainerClient(string containerName)
    {
        var connectionString = configuration.GetConnectionString("AzureStorage");
        var blobServiceClient = new BlobServiceClient(connectionString);
        return blobServiceClient.GetBlobContainerClient(containerName);
    }
}
using eShop.Storage.Api.Enums;
using eShop.Storage.Api.Interfaces;

namespace eShop.Storage.Api.Services;

internal sealed class StoreService(BlobServiceClient blobServiceClient) : IStoreService
{
    private readonly BlobServiceClient blobServiceClient = blobServiceClient;

    public async ValueTask<List<string>> FindAsync(string identifier, string type)
    {
        var containerClient = await GetClientAsync(type);
        var files = containerClient.GetBlobs(prefix: identifier);

        if (files is null || !files.Any())
        {
            return new List<string>();
        }
        
        var uris = files.Select(x => x.Name).ToList();
        return uris;
    }

    public async ValueTask<List<string>> UploadRangeAsync(IEnumerable<IFormFile> files, string type, string identifier)
    {
        var uriList = new List<string>();
        var blobs = files.ToImmutableList();
        var containerClient = await GetClientAsync(type);

        var index = 0;

        foreach (var file in blobs)
        {
            var client = containerClient.GetBlobClient($"{identifier}_{index}");
            await using var stream = blobs[index].OpenReadStream();
            await client.UploadAsync(stream, true);
            index++;

            uriList.Add(client.Uri.ToString());
        }

        return uriList;
    }

    public async ValueTask DeleteAsync(string identifier, string type)
    {
        var containerClient = await GetClientAsync(type);
        await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: identifier))
        {
            var blobClient = containerClient.GetBlobClient(blobItem.Name);
            await blobClient.DeleteIfExistsAsync();
        }
    }

    private async Task<BlobContainerClient> GetClientAsync(string type)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(type);
        await containerClient.CreateIfNotExistsAsync();
        
        return blobServiceClient.GetBlobContainerClient(type);
    }
}
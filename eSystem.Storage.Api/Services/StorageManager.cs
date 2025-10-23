using eSystem.Domain.Common.Http;
using eSystem.Storage.Api.Interfaces;

namespace eSystem.Storage.Api.Services;

internal sealed class StorageManager(BlobServiceClient blobServiceClient) : IStorageManager
{
    private readonly BlobServiceClient blobServiceClient = blobServiceClient;

    public async ValueTask<List<string>> LoadAsync(Metadata metadata)
    {
        var containerClient = await GetClientAsync(metadata.Type);
        var files = containerClient.GetBlobs(prefix: metadata.Identifier);

        if (files is null || !files.Any())
        {
            return [];
        }
        
        var uris = files.Select(x => x.Name).ToList();
        return uris;
    }

    public async ValueTask<List<string>> UploadAsync(IEnumerable<IFormFile> files, Metadata metadata)
    {
        var uriList = new List<string>();
        var blobs = files.ToImmutableList();
        var containerClient = await GetClientAsync(metadata.Type);
        
        foreach (var file in blobs)
        {
            var index = blobs.IndexOf(file);
            var client = containerClient.GetBlobClient($"{metadata.Identifier}_{index}");
            await using var stream = file.OpenReadStream();
            await client.UploadAsync(stream, true);

            uriList.Add(client.Uri.ToString());
        }

        return uriList;
    }

    public async ValueTask DeleteAsync(Metadata metadata)
    {
        var containerClient = await GetClientAsync(metadata.Type);
        await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: metadata.Identifier))
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
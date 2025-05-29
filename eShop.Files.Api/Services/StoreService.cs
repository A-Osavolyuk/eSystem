using eShop.Files.Api.Interfaces;

namespace eShop.Files.Api.Services;

internal sealed class StoreService(BlobServiceClient blobServiceClient) : IStoreService
{
    private readonly BlobServiceClient blobServiceClient = blobServiceClient;

    public async ValueTask<List<string>> FindManyAsync(string prefix, Container container)
    {
        var containerClient = await GetContainerClient(container);
        var files = containerClient.GetBlobs(prefix: prefix);

        if (files is null || !files.Any())
        {
            return new List<string>();
        }
        
        var uris = files.Select(x => x.Name).ToList();
        return uris;
    }

    public async ValueTask<List<string>> UploadRangeAsync(IEnumerable<IFormFile> files, string key, Container container)
    {
        var uriList = new List<string>();
        var blobs = files.ToImmutableList();
        var containerClient = await GetContainerClient(container);

        var index = 0;

        foreach (var file in blobs)
        {
            var client = containerClient.GetBlobClient($"{key}_{index}");
            await using var stream = blobs[index].OpenReadStream();
            await client.UploadAsync(stream, true);
            index++;

            uriList.Add(client.Uri.ToString());
        }

        return uriList;
    }

    public async ValueTask<string> UploadAsync(IFormFile file, string key, Container container)
    {
        var containerClient = await GetContainerClient(container);
        var client = containerClient.GetBlobClient($"{key}");

        await using var stream = file.OpenReadStream();
        await client.UploadAsync(stream, true);

        var uri = client.Uri.ToString();

        return uri;
    }

    public async ValueTask DeleteAsync(string prefix, Container container)
    {
        var containerClient = await GetContainerClient(container);
        await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: prefix))
        {
            var blobClient = containerClient.GetBlobClient(blobItem.Name);
            await blobClient.DeleteIfExistsAsync();
        }
    }

    public async ValueTask<string> FindAsync(string key, Container container)
    {
        var containerClient = await GetContainerClient(container);
        var blobClient = containerClient.GetBlobClient(key);
        return blobClient.Uri.ToString();
    }

    private async Task<BlobContainerClient> GetContainerClient(Container container)
    {
        var containerName = container switch
        {
            Container.Avatar => nameof(Container.Avatar),
            Container.Product => nameof(Container.Product),
            _ => throw new ArgumentOutOfRangeException(nameof(container), container, null)
        };
        
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();
        
        return blobServiceClient.GetBlobContainerClient(containerName);
    }
}
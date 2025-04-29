using eShop.Files.Api.Interfaces;

namespace eShop.Files.Api.Services;

internal sealed class StoreService(IConfiguration configuration) : IStoreService
{
    private readonly string avatarContainer = configuration["Configuration:Storage:Azure:Containers:AvatarContainer"]!;
    private readonly string productContainer = configuration["Configuration:Storage:Azure:Containers:ProductContainer"]!;
    private readonly string connectionString = configuration["Configuration:Storage:Azure:ConnectionString"]!;

    public ValueTask<List<string>> GetManyAsync(string prefix, Container container)
    {
        var containerClient = GetContainerClient(container);
        var files = containerClient.GetBlobs(prefix: prefix);

        if (files is null || !files.Any())
        {
            return ValueTask.FromResult(new List<string>());
        }
        
        var uris = files.Select(x => x.Name).ToList();
        return ValueTask.FromResult(uris);
    }

    public async ValueTask<List<string>> UploadRangeAsync(IEnumerable<IFormFile> files, string key, Container container)
    {
        var uriList = new List<string>();
        var blobs = files.ToImmutableList();

        for (var i = 0; i < blobs.Count; i++)
        {
            var containerClient = GetContainerClient(container);
            var client = containerClient.GetBlobClient($"{key}_{i}");
            await using var stream = blobs[i].OpenReadStream();
            await client.UploadAsync(stream, true);

            uriList.Add(client.Uri.ToString());
        }

        return uriList;
    }

    public async ValueTask<string> UploadAsync(IFormFile file, string key, Container container)
    {
        var containerClient = GetContainerClient(container);
        var client = containerClient.GetBlobClient($"{key}");

        await using var stream = file.OpenReadStream();
        await client.UploadAsync(stream, true);

        var uri = client.Uri.ToString();

        return uri;
    }

    public async ValueTask DeleteAsync(string prefix, Container container)
    {
        var containerClient = GetContainerClient(container);
        await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: prefix))
        {
            var blobClient = containerClient.GetBlobClient(blobItem.Name);
            await blobClient.DeleteIfExistsAsync();
        }
    }

    public ValueTask<string> GetAsync(string key, Container container)
    {
        var containerClient = GetContainerClient(container);
        var blobClient = containerClient.GetBlobClient(key);
        return ValueTask.FromResult(blobClient.Uri.ToString());
    }

    private BlobContainerClient GetContainerClient(Container container)
    {
        var containerName = container switch
        {
            Container.Avatar => avatarContainer,
            Container.Product => productContainer,
            _ => throw new ArgumentOutOfRangeException(nameof(container), container, null)
        };
        
        var blobServiceClient = new BlobServiceClient(connectionString);
        return blobServiceClient.GetBlobContainerClient(containerName);
    }
}
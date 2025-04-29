using eShop.Files.Api.Interfaces;

namespace eShop.Files.Api.Services;

internal sealed class StoreService(IConfiguration configuration) : IStoreService
{
    private readonly string avatarContainer = configuration["Configuration:Storage:Azure:Containers:AvatarContainer"]!;

    private readonly string productContainer =
        configuration["Configuration:Storage:Azure:Containers:ProductContainer"]!;

    private readonly string connectionString = configuration["Configuration:Storage:Azure:ConnectionString"]!;

    public ValueTask<List<string>> GetManyAsync(string prefix)
    {
        var container = GetContainerClient(productContainer);
        var files = container.GetBlobs(prefix: prefix);

        if (files is null || !files.Any())
        {
            return ValueTask.FromResult(new List<string>());
        }
        
        var uris = files.Select(x => x.Name).ToList();
        return ValueTask.FromResult(uris);
    }

    public async ValueTask<List<string>> UploadRangeAsync(IEnumerable<IFormFile> files, string key)
    {
        var uriList = new List<string>();
        var blobs = files.ToImmutableList();

        for (var i = 0; i < blobs.Count; i++)
        {
            var container = GetContainerClient(productContainer);
            var client = container.GetBlobClient($"{key}_{i}");
            await using var stream = blobs[i].OpenReadStream();
            await client.UploadAsync(stream, true);

            uriList.Add(client.Uri.ToString());
        }

        return uriList;
    }

    public async ValueTask<string> UploadAsync(IFormFile file, string key)
    {
        var container = GetContainerClient(productContainer);
        var client = container.GetBlobClient($"{key}");

        await using var stream = file.OpenReadStream();
        await client.UploadAsync(stream, true);

        var uri = client.Uri.ToString();

        return uri;
    }

    public async ValueTask DeleteAsync(string prefix)
    {
        var container = GetContainerClient(productContainer);
        var files = container.GetBlobs(prefix: prefix);

        if (files is not null)
        {
            foreach (var file in files)
            {
                if (file is not null)
                {
                    await container.DeleteBlobAsync(file.Name);
                }
            }
        }
    }

    public ValueTask<string> GetAsync(string key)
    {
        var blobContainerClient = GetContainerClient(avatarContainer);
        var blobClient = blobContainerClient.GetBlobClient(key);
        return ValueTask.FromResult(blobClient.Uri.ToString());
    }

    private BlobContainerClient GetContainerClient(string containerName)
    {
        var blobServiceClient = new BlobServiceClient(connectionString);
        return blobServiceClient.GetBlobContainerClient(containerName);
    }
}
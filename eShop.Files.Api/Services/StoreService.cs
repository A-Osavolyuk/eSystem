using eShop.Files.Api.Interfaces;

namespace eShop.Files.Api.Services;

internal sealed class StoreService(IConfiguration configuration) : IStoreService
{
    private readonly IConfiguration configuration = configuration;

    private readonly string avatarContainer = configuration["Configuration:Storage:Azure:Containers:AvatarContainer"]!;

    private readonly string productContainer =
        configuration["Configuration:Storage:Azure:Containers:ProductContainer"]!;

    private readonly string connectionString = configuration["Configuration:Storage:Azure:ConnectionString"]!;

    public async ValueTask<List<string>> GetProductImagesAsync(Guid productId)
    {
        var uriList = new List<string>();
        var blobContainerClient = GetContainerClient(productContainer);

        for (int i = 0; true; i++)
        {
            var blobClient = blobContainerClient.GetBlobClient($"{productId}_{i}");

            if (await blobClient.ExistsAsync())
            {
                uriList.Add(blobClient.Uri.ToString());
            }
            else
            {
                break;
            }
        }

        return uriList;
    }

    public async ValueTask<List<string>> UploadProductImagesAsync(IReadOnlyCollection<IFormFile> files, Guid productId)
    {
        var uriList = new List<string>();
        var images = files.ToImmutableList();

        for (var i = 0; i < images.Count(); i++)
        {
            var blobContainerClient = GetContainerClient(productContainer);
            var blobClient = blobContainerClient.GetBlobClient($"{productId}_{i}");
            await using (var stream = images[i].OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            uriList.Add(blobClient.Uri.ToString());
        }

        return uriList;
    }

    public async ValueTask DeleteProductImagesAsync(Guid productId)
    {
        var blobContainerClient = GetContainerClient(productContainer);
        for (int i = 0; true; i++)
        {
            var blobClient = blobContainerClient.GetBlobClient($"{productId}_{i}");

            if (await blobClient.ExistsAsync())
                await blobClient.DeleteAsync();
            else
                break;
        }
    }

    public async ValueTask<string> UploadUserAvatarAsync(IFormFile file, Guid userId)
    {
        var blobContainerClient = GetContainerClient(avatarContainer);
        var blobClient = blobContainerClient.GetBlobClient($"avatar_{userId}");
        await using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream);
        return blobClient.Uri.ToString();
    }

    public async ValueTask DeleteUserAvatarAsync(Guid userId)
    {
        var blobContainerClient = GetContainerClient(avatarContainer);
        var blobClient = blobContainerClient.GetBlobClient($"avatar_{userId}");
        if (await blobClient.ExistsAsync())
        {
            await blobClient.DeleteAsync();
        }
    }

    public ValueTask<string> GetUserAvatarAsync(Guid userId)
    {
        var blobContainerClient = GetContainerClient(avatarContainer);
        var blobClient = blobContainerClient.GetBlobClient($"avatar_{userId}");
        return ValueTask.FromResult(blobClient.Uri.ToString());
    }

    private BlobContainerClient GetContainerClient(string containerName)
    {
        var blobServiceClient = new BlobServiceClient(connectionString);
        return blobServiceClient.GetBlobContainerClient(containerName);
    }
}
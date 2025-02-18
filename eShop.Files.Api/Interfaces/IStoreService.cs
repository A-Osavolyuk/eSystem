namespace eShop.Files.Api.Interfaces;

public interface IStoreService
{
    public ValueTask<List<string>> GetProductImagesAsync(Guid productId);
    public ValueTask<List<string>> UploadProductImagesAsync(IReadOnlyCollection<IFormFile> files, Guid productId);
    public ValueTask DeleteProductImagesAsync(Guid productId);
    public ValueTask<string> UploadUserAvatarAsync(IFormFile file, Guid userId);
    public ValueTask DeleteUserAvatarAsync(Guid userId);
    public ValueTask<string> GetUserAvatarAsync(Guid userId);
}
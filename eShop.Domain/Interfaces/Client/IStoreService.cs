using eShop.Domain.Common.API;

namespace eShop.Domain.Interfaces.Client;

public interface IStoreService
{
    public ValueTask<Response> RemoveUserAvatarAsync(string userId);
    public ValueTask<Response> UploadUserAvatarAsync(string userId, IBrowserFile file);
    public ValueTask<Response> GetUserAvatarAsync(string userId);
    public ValueTask<Response> UploadProductImagesAsync(IReadOnlyList<IBrowserFile> files, Guid productId);
}
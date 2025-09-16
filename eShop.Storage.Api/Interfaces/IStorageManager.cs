using eShop.Domain.Common.Http;

namespace eShop.Storage.Api.Interfaces;

public interface IStorageManager
{
    public ValueTask<List<string>> LoadAsync(Metadata metadata);
    public ValueTask<List<string>> UploadAsync(IEnumerable<IFormFile> files, Metadata metadata);
    public ValueTask DeleteAsync(Metadata metadata);
}
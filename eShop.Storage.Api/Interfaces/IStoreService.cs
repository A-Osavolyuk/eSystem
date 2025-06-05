using eShop.Storage.Api.Enums;

namespace eShop.Storage.Api.Interfaces;

public interface IStoreService
{
    public ValueTask<string> FindAsync(string key, Container container);
    public ValueTask<List<string>> FindManyAsync(string prefix, Container container);
    public ValueTask<List<string>> UploadRangeAsync(IEnumerable<IFormFile> files, string key, Container container);
    public ValueTask<string> UploadAsync(IFormFile file, string key, Container container);
    public ValueTask DeleteAsync(string prefix, Container container);
}
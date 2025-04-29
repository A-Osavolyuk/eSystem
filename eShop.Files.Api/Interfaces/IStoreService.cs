namespace eShop.Files.Api.Interfaces;

public interface IStoreService
{
    public ValueTask<string> GetAsync(string key, Container container);
    public ValueTask<List<string>> GetManyAsync(string prefix, Container container);
    public ValueTask<List<string>> UploadRangeAsync(IEnumerable<IFormFile> files, string key, Container container);
    public ValueTask<string> UploadAsync(IFormFile file, string key, Container container);
    public ValueTask DeleteAsync(string prefix, Container container);
}
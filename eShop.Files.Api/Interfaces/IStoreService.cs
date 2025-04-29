namespace eShop.Files.Api.Interfaces;

public interface IStoreService
{
    public ValueTask<string> GetAsync(string key);
    public ValueTask<List<string>> GetManyAsync(string prefix);
    public ValueTask<List<string>> UploadRangeAsync(IEnumerable<IFormFile> files, string key);
    public ValueTask<string> UploadAsync(IFormFile file, string key);
    public ValueTask DeleteAsync(string prefix);
}
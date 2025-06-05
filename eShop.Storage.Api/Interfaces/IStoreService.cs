using eShop.Storage.Api.Enums;

namespace eShop.Storage.Api.Interfaces;

public interface IStoreService
{
    public ValueTask<List<string>> FindAsync(string identifier, string type);
    public ValueTask<List<string>> UploadRangeAsync(IEnumerable<IFormFile> files, string type, string identifier);
    public ValueTask DeleteAsync(string identifier, string type);
}
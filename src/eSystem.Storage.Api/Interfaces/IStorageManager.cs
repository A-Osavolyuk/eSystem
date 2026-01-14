using eSystem.Core.Requests;

namespace eSystem.Storage.Api.Interfaces;

public interface IStorageManager
{
    public ValueTask<List<string>> LoadAsync(Metadata metadata);
    public ValueTask<List<string>> UploadAsync(IEnumerable<IFormFile> files, Metadata metadata);
    public ValueTask DeleteAsync(Metadata metadata);
}
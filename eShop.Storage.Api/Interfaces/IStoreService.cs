using eShop.Domain.Common.API;
using eShop.Storage.Api.Enums;

namespace eShop.Storage.Api.Interfaces;

public interface IStoreService
{
    public ValueTask<List<string>> DownloadAsync(Metadata metadata);
    public ValueTask<List<string>> UploadAsync(IEnumerable<IFormFile> files, Metadata metadata);
    public ValueTask DeleteAsync(Metadata metadata);
}
using eSystem.Core.Requests.Storage;

namespace eAccount.Common.Storage.External;

public interface IStorageService
{
    public ValueTask<HttpResponse> UploadFilesAsync(UploadFilesRequest request);
    public ValueTask<HttpResponse> LoadFilesAsync(LoadFilesRequest request);
}
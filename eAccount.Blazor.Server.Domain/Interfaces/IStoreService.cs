using eSystem.Domain.Common.Http;
using eSystem.Domain.Requests.Storage;

namespace eAccount.Blazor.Server.Domain.Interfaces;

public interface IStoreService
{
    public ValueTask<HttpResponse> UploadFilesAsync(UploadFilesRequest request);
    public ValueTask<HttpResponse> LoadFilesAsync(LoadFilesRequest request);
}
using eSystem.Core.Common.Http;
using eSystem.Core.Requests.Storage;

namespace eAccount.Blazor.Server.Domain.Interfaces;

public interface IStoreService
{
    public ValueTask<HttpResponse> UploadFilesAsync(UploadFilesRequest request);
    public ValueTask<HttpResponse> LoadFilesAsync(LoadFilesRequest request);
}
using eShop.Domain.Common.Http;
using eShop.Domain.Requests.API.Storage;

namespace eShop.Blazor.Server.Domain.Interfaces;

public interface IStoreService
{
    public ValueTask<HttpResponse> UploadFilesAsync(UploadFilesRequest request);
    public ValueTask<HttpResponse> LoadFilesAsync(LoadFilesRequest request);
}
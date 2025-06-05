using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Storage;

namespace eShop.Domain.Interfaces.Client;

public interface IStoreService
{
    public ValueTask<Response> UploadFilesAsync(UploadFilesRequest request);
}
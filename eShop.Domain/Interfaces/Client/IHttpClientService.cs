using eShop.Domain.Common.API;

namespace eShop.Domain.Interfaces.Client;

public interface IHttpClientService
{
    public ValueTask<Response> SendAsync(Request request, bool withBearer = true);
    public ValueTask<Response> SendAsync(FileRequest request, bool withBearer = true);
}
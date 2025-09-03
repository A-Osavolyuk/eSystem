using eShop.Domain.Common.API;
using eShop.Domain.Common.Http;

namespace eShop.Domain.Interfaces.Client;

public interface ICategoryService
{
    public ValueTask<HttpResponse> GetAllAsync();
}
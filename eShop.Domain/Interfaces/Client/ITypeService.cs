using eShop.Domain.Common.API;
using eShop.Domain.Common.Http;

namespace eShop.Domain.Interfaces.Client;

public interface ITypeService
{
    public ValueTask<HttpResponse> GetAllAsync();
}
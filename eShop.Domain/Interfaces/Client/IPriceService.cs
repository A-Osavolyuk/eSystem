using eShop.Domain.Common.API;
using eShop.Domain.Common.Http;

namespace eShop.Domain.Interfaces.Client;

public interface IPriceService
{
    public ValueTask<Response> GetAllAsync();
}
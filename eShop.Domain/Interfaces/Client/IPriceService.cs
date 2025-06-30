using eShop.Domain.Common.API;

namespace eShop.Domain.Interfaces.Client;

public interface IPriceService
{
    public ValueTask<Response> GetAllAsync();
}
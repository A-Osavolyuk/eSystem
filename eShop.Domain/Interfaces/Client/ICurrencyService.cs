using eShop.Domain.Common.API;

namespace eShop.Domain.Interfaces.Client;

public interface ICurrencyService
{
    public ValueTask<Response> GetAllAsync();
}
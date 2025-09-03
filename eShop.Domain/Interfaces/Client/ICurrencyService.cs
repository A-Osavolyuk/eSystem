using eShop.Domain.Common.API;
using eShop.Domain.Common.Http;

namespace eShop.Domain.Interfaces.Client;

public interface ICurrencyService
{
    public ValueTask<Response> GetAllAsync();
}
using eShop.Domain.Common.API;

namespace eShop.Domain.Interfaces.Client;

public interface IUnitService
{
    public ValueTask<Response> GetAllAsync();
}
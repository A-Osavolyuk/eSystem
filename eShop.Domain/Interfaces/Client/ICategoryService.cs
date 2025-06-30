using eShop.Domain.Common.API;

namespace eShop.Domain.Interfaces.Client;

public interface ICategoryService
{
    public ValueTask<Response> GetAllAsync();
}
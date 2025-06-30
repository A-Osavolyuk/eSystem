using eShop.Domain.Common.API;

namespace eShop.Domain.Interfaces.Client;

public interface ITypeService
{
    public ValueTask<Response> GetAllAsync();
}
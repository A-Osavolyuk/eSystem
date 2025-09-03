using eShop.Domain.Common.Http;

namespace eShop.Blazor.Domain.Interfaces;

public interface IUnitService
{
    public ValueTask<HttpResponse> GetAllAsync();
}
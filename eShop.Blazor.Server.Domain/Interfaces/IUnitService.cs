using eShop.Domain.Common.Http;

namespace eShop.Blazor.Server.Domain.Interfaces;

public interface IUnitService
{
    public ValueTask<HttpResponse> GetAllAsync();
}
using eShop.Domain.Common.Http;

namespace eShop.Blazor.Domain.Interfaces;

public interface IPriceService
{
    public ValueTask<HttpResponse> GetAllAsync();
}
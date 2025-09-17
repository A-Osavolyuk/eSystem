using eShop.Domain.Common.Http;

namespace eShop.Blazor.Server.Domain.Interfaces;

public interface IPriceService
{
    public ValueTask<HttpResponse> GetAllAsync();
}
using eShop.Domain.Common.Http;

namespace eShop.Blazor.Server.Domain.Interfaces;

public interface ICurrencyService
{
    public ValueTask<HttpResponse> GetAllAsync();
}
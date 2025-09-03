using eShop.Domain.Common.Http;

namespace eShop.Blazor.Domain.Interfaces;

public interface ICurrencyService
{
    public ValueTask<HttpResponse> GetAllAsync();
}
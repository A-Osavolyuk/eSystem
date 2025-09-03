using eShop.Domain.Common.Http;

namespace eShop.Blazor.Domain.Interfaces;

public interface ICategoryService
{
    public ValueTask<HttpResponse> GetAllAsync();
}
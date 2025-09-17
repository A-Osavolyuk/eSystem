using eShop.Domain.Common.Http;

namespace eShop.Blazor.Server.Domain.Interfaces;

public interface ICategoryService
{
    public ValueTask<HttpResponse> GetAllAsync();
}
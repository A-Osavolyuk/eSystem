using eShop.Domain.Common.Http;

namespace eShop.Blazor.Domain.Interfaces;

public interface ITypeService
{
    public ValueTask<HttpResponse> GetAllAsync();
}
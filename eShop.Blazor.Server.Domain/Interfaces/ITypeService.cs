using eShop.Domain.Common.Http;

namespace eShop.Blazor.Server.Domain.Interfaces;

public interface ITypeService
{
    public ValueTask<HttpResponse> GetAllAsync();
}
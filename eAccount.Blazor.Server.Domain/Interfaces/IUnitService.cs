using eShop.Domain.Common.Http;

namespace eAccount.Blazor.Server.Domain.Interfaces;

public interface IUnitService
{
    public ValueTask<HttpResponse> GetAllAsync();
}
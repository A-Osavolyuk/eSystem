using eSystem.Domain.Common.Http;

namespace eAccount.Blazor.Server.Domain.Interfaces;

public interface IPriceService
{
    public ValueTask<HttpResponse> GetAllAsync();
}
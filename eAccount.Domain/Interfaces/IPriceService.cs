using eSystem.Core.Common.Http;

namespace eAccount.Domain.Interfaces;

public interface IPriceService
{
    public ValueTask<HttpResponse> GetAllAsync();
}
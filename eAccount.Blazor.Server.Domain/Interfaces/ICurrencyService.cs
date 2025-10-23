using eSystem.Domain.Common.Http;

namespace eAccount.Blazor.Server.Domain.Interfaces;

public interface ICurrencyService
{
    public ValueTask<HttpResponse> GetAllAsync();
}
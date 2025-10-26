using eSystem.Core.Common.Http;

namespace eAccount.Domain.Interfaces;

public interface ICurrencyService
{
    public ValueTask<HttpResponse> GetAllAsync();
}
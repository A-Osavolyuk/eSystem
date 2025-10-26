using eSystem.Core.Common.Http;

namespace eAccount.Domain.Interfaces;

public interface IUnitService
{
    public ValueTask<HttpResponse> GetAllAsync();
}
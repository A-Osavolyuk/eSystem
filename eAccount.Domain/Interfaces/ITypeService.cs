using eSystem.Core.Common.Http;

namespace eAccount.Domain.Interfaces;

public interface ITypeService
{
    public ValueTask<HttpResponse> GetAllAsync();
}
using eSystem.Domain.Common.Http;

namespace eAccount.Blazor.Server.Domain.Interfaces;

public interface ITypeService
{
    public ValueTask<HttpResponse> GetAllAsync();
}
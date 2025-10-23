using eSystem.Domain.Common.Http;

namespace eAccount.Blazor.Server.Domain.Interfaces;

public interface ICategoryService
{
    public ValueTask<HttpResponse> GetAllAsync();
}
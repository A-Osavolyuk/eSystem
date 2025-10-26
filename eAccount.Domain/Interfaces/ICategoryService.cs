using eSystem.Core.Common.Http;

namespace eAccount.Domain.Interfaces;

public interface ICategoryService
{
    public ValueTask<HttpResponse> GetAllAsync();
}
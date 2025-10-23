using eShop.Domain.Common.Http;
using eShop.Domain.Requests.Auth;

namespace eAccount.Blazor.Server.Domain.Interfaces;

public interface IPasskeyService
{
    public ValueTask<HttpResponse> GetAsync(Guid id);
    public ValueTask<HttpResponse> ChangeNameAsync(ChangePasskeyNameRequest request);
    public ValueTask<HttpResponse> GenerateCreationOptionsAsync(GenerateCreationOptionsRequest request);
    public ValueTask<HttpResponse> CreatAsync(CreatePasskeyRequest request);
    public ValueTask<HttpResponse> GenerateRequestOptionsAsync(GenerateRequestOptionsRequest requestOptionsRequest);
    public ValueTask<HttpResponse> RemoveAsync(RemovePasskeyRequest request);
}
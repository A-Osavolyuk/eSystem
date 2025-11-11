using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Services.Interfaces;

public interface IPasskeyService
{
    public ValueTask<HttpResponse> GenerateRequestOptionsAsync(GenerateRequestOptionsRequest request);
    public ValueTask<HttpResponse> GenerateCreationOptionsAsync(GenerateCreationOptionsRequest request);
    public ValueTask<HttpResponse> CreateAsync(CreatePasskeyRequest request);
    public ValueTask<HttpResponse> ChangeNameAsync(ChangePasskeyNameRequest request);
    public ValueTask<HttpResponse> RemoveAsync(RemovePasskeyRequest request);
}
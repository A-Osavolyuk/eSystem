using eSystem.Core.Requests.Auth;

namespace eAccount.Security.Credentials.PublicKey;

public interface IPasskeyService
{
    public ValueTask<HttpResponse> ChangeNameAsync(ChangePasskeyNameRequest request);
    public ValueTask<HttpResponse> GenerateCreationOptionsAsync(GenerateCreationOptionsRequest request);
    public ValueTask<HttpResponse> CreatAsync(CreatePasskeyRequest request);
    public ValueTask<HttpResponse> GenerateRequestOptionsAsync(GenerateRequestOptionsRequest request);
    public ValueTask<HttpResponse> RemoveAsync(RemovePasskeyRequest request);
}
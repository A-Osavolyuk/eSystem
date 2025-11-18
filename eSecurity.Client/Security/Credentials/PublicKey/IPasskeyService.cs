using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Credentials.PublicKey;
using eSystem.Core.Common.Http;

namespace eSecurity.Client.Security.Credentials.PublicKey;

public interface IPasskeyService
{
    public ValueTask<HttpResponse<PublicKeyCredentialRequestOptions>> GenerateRequestOptionsAsync(
        GenerateRequestOptionsRequest request);
    public ValueTask<HttpResponse<PublicKeyCredentialCreationOptions>> GenerateCreationOptionsAsync(
        GenerateCreationOptionsRequest request);
    
    public ValueTask<HttpResponse> CreateAsync(CreatePasskeyRequest request);
    public ValueTask<HttpResponse> ChangeNameAsync(ChangePasskeyNameRequest request);
    public ValueTask<HttpResponse> RemoveAsync(RemovePasskeyRequest request);
}
using eSecurity.Client.Common.Http;
using eSecurity.Core.Requests;

namespace eSecurity.Client.Security.Credentials.PublicKey;

public interface ISoftwareService
{
    public ValueTask<ApiResponse> GenerateRequestOptionsAsync(GenerateRequestOptionsRequest request);
    public ValueTask<ApiResponse> GenerateCreationOptionsAsync(GenerateCreationOptionsRequest request);
    public ValueTask<ApiResponse> CreateAsync(CreateSoftwareKeyRequest request);
    public ValueTask<ApiResponse> ChangeNameAsync(ChangeSoftwareKeyNameRequest request);
    public ValueTask<ApiResponse> RemoveAsync(RemoveSoftwareKeyRequest request);
}
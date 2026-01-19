using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Credentials.PublicKey;

public interface IPasskeyService
{
    public ValueTask<ApiResponse> GenerateRequestOptionsAsync(GenerateRequestOptionsRequest request);
    public ValueTask<ApiResponse> GenerateCreationOptionsAsync(GenerateCreationOptionsRequest request);
    public ValueTask<ApiResponse> CreateAsync(CreatePasskeyRequest request);
    public ValueTask<ApiResponse> ChangeNameAsync(ChangePasskeyNameRequest request);
    public ValueTask<ApiResponse> RemoveAsync(RemovePasskeyRequest request);
}
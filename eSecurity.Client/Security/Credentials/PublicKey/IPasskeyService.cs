using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Credentials.PublicKey;

public interface IPasskeyService
{
    public ValueTask<Result> GenerateRequestOptionsAsync(GenerateRequestOptionsRequest request);
    public ValueTask<Result> GenerateCreationOptionsAsync(GenerateCreationOptionsRequest request);
    public ValueTask<Result> CreateAsync(CreatePasskeyRequest request);
    public ValueTask<Result> ChangeNameAsync(ChangePasskeyNameRequest request);
    public ValueTask<Result> RemoveAsync(RemovePasskeyRequest request);
}
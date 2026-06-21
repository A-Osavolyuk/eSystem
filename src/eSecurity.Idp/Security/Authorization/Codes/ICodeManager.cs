using eSecurity.Idp.Data.Entities;
using eSystem.Core.Messaging;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authorization.Codes;

public interface ICodeManager
{
    public ValueTask<CodeEntity?> FindByCodeAsync(UserEntity user, 
        string code, CancellationToken cancellationToken = default);
    
    public ValueTask<TypedResult<string>> CreateAsync(UserEntity user, 
        SenderType sender, CancellationToken cancellationToken = default);

    public ValueTask<Result> ConsumeAsync(CodeEntity code, CancellationToken cancellationToken = default);
    public ValueTask<Result> CancelAsync(CodeEntity code, CancellationToken cancellationToken = default);
}
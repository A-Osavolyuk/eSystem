using eSecurity.Server.Data.Entities;
using eSystem.Core.Common.Messaging;
using eSystem.Core.Primitives;

namespace eSecurity.Server.Security.Authorization.Codes;

public interface ICodeManager
{
    public ValueTask<CodeEntity?> FindAsync(UserEntity user, 
        string code, CancellationToken cancellationToken = default);
    
    public ValueTask<string> CreateAsync(UserEntity user, 
        SenderType sender, CancellationToken cancellationToken = default);

    public ValueTask<Result> ConsumeAsync(CodeEntity code, CancellationToken cancellationToken = default);
    public ValueTask<Result> CancelAsync(CodeEntity code, CancellationToken cancellationToken = default);
}
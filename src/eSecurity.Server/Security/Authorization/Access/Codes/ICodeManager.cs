using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Server.Data.Entities;
using eSystem.Core.Common.Messaging;

namespace eSecurity.Server.Security.Authorization.Access.Codes;

public interface ICodeManager
{
    public ValueTask<CodeEntity?> FindAsync(UserEntity user, 
        string codeHash, CancellationToken cancellationToken = default);
    
    public ValueTask<string> GenerateAsync(UserEntity user, SenderType sender, ActionType action, 
        PurposeType purpose, CancellationToken cancellationToken = default);

    public ValueTask<Result> RemoveAsync(CodeEntity code, CancellationToken cancellationToken = default);
}
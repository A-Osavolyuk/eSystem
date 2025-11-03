using eSecurity.Data.Entities;
using eSystem.Core.Common.Messaging;
using eSystem.Core.Security.Authorization.Access;

namespace eSecurity.Security.Authorization.Access;

public interface ICodeManager
{
    public ValueTask<string> GenerateAsync(UserEntity user, SenderType sender, ActionType action, 
        PurposeType purpose, CancellationToken cancellationToken = default);

    public ValueTask<Result> VerifyAsync(UserEntity user, string code, SenderType sender, ActionType action,
        PurposeType purpose, CancellationToken cancellationToken = default);
}
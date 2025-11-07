using eSecurity.Data.Entities;
using eSystem.Core.Common.Messaging;

namespace eSecurity.Security.Authorization.Access.Codes;

public interface ICodeManager
{
    public ValueTask<string> GenerateAsync(UserEntity user, SenderType sender, ActionType action, 
        PurposeType purpose, CancellationToken cancellationToken = default);

    public ValueTask<Result> VerifyAsync(UserEntity user, string code, SenderType sender, ActionType action,
        PurposeType purpose, CancellationToken cancellationToken = default);
}
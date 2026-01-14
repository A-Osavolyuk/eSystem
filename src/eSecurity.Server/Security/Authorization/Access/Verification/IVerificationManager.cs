using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authorization.Access.Verification;

public interface IVerificationManager
{
    public ValueTask<Result> CreateAsync(UserEntity user, PurposeType purpose,
        ActionType action, CancellationToken cancellationToken = default);

    public ValueTask<Result> VerifyAsync(UserEntity user, PurposeType purpose,
        ActionType action, CancellationToken cancellationToken = default);
}
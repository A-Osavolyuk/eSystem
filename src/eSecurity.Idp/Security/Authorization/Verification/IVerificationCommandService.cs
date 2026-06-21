using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authorization.Verification;

public interface IVerificationCommandService
{
    ValueTask<Result> CreateAsync(VerificationRequestEntity entity, CancellationToken cancellationToken = default);

    ValueTask<Result> ConsumeAsync(VerificationRequestEntity entity, CancellationToken cancellationToken = default);

    ValueTask<Result> ApproveAsync(VerificationRequestEntity entity, CancellationToken cancellationToken = default);

    ValueTask<Result> CancelAsync(VerificationRequestEntity entity, CancellationToken cancellationToken = default);
}
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authorization.Verification.Extensions;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Authorization.Verification;

public sealed class VerificationCommandService(
    AuthDbContext context,
    IVerificationPolicy policy) : IVerificationCommandService
{
    private readonly AuthDbContext _context = context;
    private readonly IVerificationPolicy _policy = policy;

    public async ValueTask<Result> CreateAsync(VerificationRequestEntity entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _context.VerificationRequests.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Created);
    }

    public async ValueTask<Result> ConsumeAsync(VerificationRequestEntity entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var requestInfo = entity.ToInfo();
        var canConsumeResult = _policy.CanConsume(requestInfo);
        if (!canConsumeResult.Succeeded) return canConsumeResult;
        
        entity.Status = VerificationStatus.Consumed;

        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> ApproveAsync(VerificationRequestEntity entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var requestInfo = entity.ToInfo();
        var canApproveResult = _policy.CanApprove(requestInfo);
        if (!canApproveResult.Succeeded) return canApproveResult;
        
        entity.Status = VerificationStatus.Approved;

        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> CancelAsync(VerificationRequestEntity entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var requestInfo = entity.ToInfo();
        var canCancelResult = _policy.CanCancel(requestInfo);
        if (!canCancelResult.Succeeded) return canCancelResult;
        
        entity.Status = VerificationStatus.Approved;

        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }
}
using eSecurity.Core.Security.Authentication.TwoFactor;
using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.TwoFactor.Extensions;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Authentication.TwoFactor;

public sealed class TwoFactorCommandService(
    AuthDbContext context,
    ITwoFactorQueryService query,
    ITwoFactorPolicy policy) : ITwoFactorCommandService
{
    private readonly AuthDbContext _context = context;
    private readonly ITwoFactorQueryService _query = query;
    private readonly ITwoFactorPolicy _policy = policy;

    public async ValueTask<Result> AddMethodAsync(Guid userId, TwoFactorMethod method,
        CancellationToken cancellationToken = default)
    {
        var existingMethods = await _query.ListByUserAsync(userId, cancellationToken);
        var methodsInfo = existingMethods.Select(x => x.ToInfo()).ToList();
        var canAddMethodResult = _policy.CanAddMethod(methodsInfo, method);
        if (!canAddMethodResult.Succeeded)
            return canAddMethodResult;

        var twoFactorMethod = await _context.TwoFactorMethods
            .FirstOrDefaultAsync(x => x.Type == method, cancellationToken);

        if (twoFactorMethod is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid 2FA method"
            });
        }

        var methodEntity = new UserTwoFactorMethodEntity()
        {
            Id = Guid.CreateVersion7(),
            MethodId = twoFactorMethod.Id,
            UserId = userId,
            Preferred = existingMethods.Count == 0
        };

        await _context.UserTwoFactorMethods.AddAsync(methodEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Created);
    }

    public async ValueTask<Result> RemoveMethodAsync(Guid userId, Guid methodId,
        CancellationToken cancellationToken = default)
    {
        var existingMethods = await _query.ListByUserAsync(userId, cancellationToken);
        if (existingMethods.Count == 0)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "2FA is not enabled"
            });
        }
        
        var existingMethod = existingMethods.FirstOrDefault(x => x.Id == methodId);
        if (existingMethod is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid 2FA method"
            });
        }

        var remainingMethods = existingMethods.Where(x => x.Method != existingMethod.Method).ToList();
        if (remainingMethods.Count > 0)
        {
            var highestPriorityMethod = remainingMethods.MaxBy(x => x.Method.Priority);
            if (highestPriorityMethod is null)
                throw new InvalidOperationException("Highest priority method is invalid");
            
            highestPriorityMethod.Preferred = true;
        }

        _context.UserTwoFactorMethods.Remove(existingMethod);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> SetPreferredMethodAsync(Guid userId, Guid methodId,
        CancellationToken cancellationToken = default)
    {
        var existingMethods = await _query.ListByUserAsync(userId, cancellationToken);
        if (existingMethods.Count == 0)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "2FA is not enabled"
            });
        }
        
        var existingMethod = existingMethods.FirstOrDefault(x => x.Id == methodId);
        if (existingMethod is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid 2FA method"
            });
        }

        var methodsInfo = existingMethods.Select(x => x.ToInfo()).ToList();
        var methodInfo = existingMethod.ToInfo();
        var canChangePreferredMethodResult = _policy.CanSetPreferredMethod(methodsInfo, methodInfo);
        if (!canChangePreferredMethodResult.Succeeded)
            return canChangePreferredMethodResult;

        var currentPreferredMethod = existingMethods.FirstOrDefault(x => x.Preferred);
        currentPreferredMethod?.Preferred = false;
        existingMethod.Preferred = true;

        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> ResetMethodsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var existingMethods = await _query.ListByUserAsync(userId, cancellationToken);
        if (existingMethods.Count == 0)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "2FA is not enabled"
            });
        }
        
        _context.UserTwoFactorMethods.RemoveRange(existingMethods);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }
}
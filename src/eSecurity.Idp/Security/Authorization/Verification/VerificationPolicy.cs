using eSecurity.Core.Security.Authorization.Verification;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Authorization.Verification;

public sealed class VerificationPolicy : IVerificationPolicy
{
    public Result CanConsume(VerificationRequestInfo request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Status != VerificationStatus.Pending || request.Status != VerificationStatus.Approved)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid verification request status"
            });
        }

        if (request.ExpiredAt < DateTimeOffset.UtcNow)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Verification request is already expired"
            });
        }

        return Results.Success(SuccessCodes.Ok);
    }

    public Result CanApprove(VerificationRequestInfo request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Status != VerificationStatus.Pending)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid verification request status"
            });
        }
        
        if (request.ExpiredAt < DateTimeOffset.UtcNow)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Verification request is already expired"
            });
        }

        return Results.Success(SuccessCodes.Ok);
    }

    public Result CanCancel(VerificationRequestInfo request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Status == VerificationStatus.Cancelled)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Cannot cancel already canceled verification request"
            });
        }
        
        if (request.Status == VerificationStatus.Consumed)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Cannot cancel already consumed verification request"
            });
        }
        
        if (request.ExpiredAt < DateTimeOffset.UtcNow)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Verification request is already expired"
            });
        }

        return Results.Success(SuccessCodes.Ok);
    }
}
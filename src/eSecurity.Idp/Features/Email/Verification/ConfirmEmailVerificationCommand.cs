using eSecurity.Core.Requests.Email;
using eSecurity.Core.Requests.Email.Verification;
using eSecurity.Idp.Security.Authorization.Codes;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Email.Verification;

public sealed record ConfirmEmailVerificationCommand(ConfirmEmailVerificationRequest Request) : IRequest<Result>;

public sealed class ConfirmEmailVerificationCommandHandler(
    IUserManager userManager,
    IEmailManager emailManager,
    ICodeManager codeManager) : IRequestHandler<ConfirmEmailVerificationCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly ICodeManager _codeManager = codeManager;

    public async Task<Result> Handle(ConfirmEmailVerificationCommand request,
        CancellationToken cancellationToken = default)
    {
        var userResult = await _userManager.GetUserAsync(cancellationToken);
        if (!userResult.Succeeded)
        {
            var error = userResult.GetError();
            return Results.ClientError(ClientErrorCode.Unauthorized, error);
        }

        if (!userResult.TryGetValue(out var user))
        {
            return Results.ClientError(ClientErrorCode.Unauthorized, new Error()
            {
                Code = ErrorCode.Unauthorized,
                Description = "Unauthorized"
            });
        }

        var email = request.Request.Email;
        if (string.IsNullOrEmpty(email))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "Email is required"
            });
        }

        var ownEmail = await _emailManager.OwnAsync(user, email, cancellationToken);
        if (!ownEmail)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "Email is invalid"
            });
        }

        var code = request.Request.Code;
        if (string.IsNullOrEmpty(code))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "Code is required"
            });
        }

        var codeEntity = await _codeManager.FindAsync(user, code, cancellationToken);
        if (codeEntity is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "Code is Invalid"
            });
        }

        var consumeResult = await _codeManager.ConsumeAsync(codeEntity, cancellationToken);
        if (!consumeResult.Succeeded) return consumeResult;

        var verificationResult = await _emailManager.VerifyAsync(user, email, cancellationToken);
        if (!verificationResult.Succeeded) return verificationResult;

        return Results.Success(SuccessCodes.Ok);
    }
}
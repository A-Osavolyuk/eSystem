using eSecurity.Core.Requests.Verification;
using eSecurity.Core.Responses.Verification;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authorization.Codes;
using eSecurity.Idp.Security.Authorization.Verification;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Verification.EmailOtp;

public sealed record VerifyEmailOtpCommand(VerifyEmailOtpRequest Request) : IRequest<Result>;

public sealed class VerifyEmailOtpCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager,
    IVerificationManager verificationManager,
    IOptions<VerificationConfiguration> options) : IRequestHandler<VerifyEmailOtpCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ICodeManager _codeManager = codeManager;
    private readonly IVerificationManager _verificationManager = verificationManager;
    private readonly VerificationConfiguration _configuration = options.Value;

    public async Task<Result> Handle(VerifyEmailOtpCommand request, CancellationToken cancellationToken = default)
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

        var code = await _codeManager.FindAsync(user, request.Request.Code, cancellationToken);
        if (code is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "Code is invalid"
            });
        }

        var codeResult = await _codeManager.ConsumeAsync(code, cancellationToken);
        if (!codeResult.Succeeded) return codeResult;

        var verificationRequest = new VerificationRequestEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Action = request.Request.Action,
            Purpose = request.Request.Purpose,
            Method = VerificationMethod.EmailOtp,
            Status = VerificationStatus.Pending,
            ExpiredAt = DateTimeOffset.UtcNow.Add(_configuration.Timestamp)
        };

        var verificationResult = await _verificationManager.CreateAsync(verificationRequest, cancellationToken);
        if (!verificationResult.Succeeded) return verificationResult;

        var response = new VerificationResponse { VerificationId = verificationRequest.Id };
        return Results.Success(SuccessCodes.Ok, response);
    }
}
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authorization.Codes;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.Core.Responses;
using eSecurity.Core.Security.Authorization.Verification;
using eSystem.Core.Messaging;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Idp.Security.Authorization.Verification.Totp;

public sealed class TotpVerificationStrategy(
    IUserManager userManager,
    ICodeManager codeManager,
    IVerificationManager verificationManager,
    IOptions<VerificationConfiguration> options) : IVerificationStrategy
{
    private readonly IUserManager _userManager = userManager;
    private readonly ICodeManager _codeManager = codeManager;
    private readonly IVerificationManager _verificationManager = verificationManager;
    private readonly VerificationConfiguration _configuration = options.Value;

    public async ValueTask<Result> ExecuteAsync(VerificationContext context, 
        CancellationToken cancellationToken = default)
    {
        if (context is not TotpVerificationContext totpContext)
            throw new InvalidOperationException("Invalid context type");
        
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

        var code = await _codeManager.FindAsync(user, totpContext.Code, cancellationToken);
        if (code is null) return Results.ClientError(ClientErrorCode.BadRequest, new Error
        {
            Code = ErrorCode.BadRequest,
            Description = "Invalid code"
        });
        
        var codeResult = await _codeManager.ConsumeAsync(code, cancellationToken);
        if (!codeResult.Succeeded) return codeResult;

        var method = code.Sender switch
        {
            SenderType.Email => VerificationMethod.EmailOtp,
            SenderType.Sms => VerificationMethod.SmsOtp,
            _ => throw new NotSupportedException("Unknown sender")
        };
        
        var requestEntity = new VerificationRequestEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Action = totpContext.Action,
            Purpose = totpContext.Purpose,
            Method = method,
            Status = VerificationStatus.Approved,
            ApprovedAt = DateTimeOffset.UtcNow,
            ExpiredAt = DateTimeOffset.UtcNow.Add(_configuration.Timestamp)
        };

        var verificationResult = await _verificationManager.CreateAsync(requestEntity, cancellationToken);
        if (!verificationResult.Succeeded) return verificationResult;

        var response = new VerificationResponse { VerificationId = requestEntity.Id };
        return Results.Success(SuccessCodes.Ok, response);
    }
}
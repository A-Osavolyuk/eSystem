using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authorization.Codes;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Common.Messaging;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Security.Authorization.Verification.Totp;

public sealed class TotpVerificationStrategy(
    IHttpContextAccessor httpContextAccessor,
    IUserManager userManager,
    ICodeManager codeManager,
    IVerificationManager verificationManager,
    IOptions<VerificationConfiguration> options) : IVerificationStrategy<TotpVerificationContext>
{
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly IUserManager _userManager = userManager;
    private readonly ICodeManager _codeManager = codeManager;
    private readonly IVerificationManager _verificationManager = verificationManager;
    private readonly VerificationConfiguration _configuration = options.Value;

    public async ValueTask<Result> ExecuteAsync(TotpVerificationContext context, 
        CancellationToken cancellationToken = default)
    {
        var subjectClaim = _httpContext.User.FindFirst(AppClaimTypes.Sub);
        if (subjectClaim is null) return Results.BadRequest("Invalid request");

        var user = await _userManager.FindBySubjectAsync(subjectClaim.Value, cancellationToken);
        if (user is null) return Results.NotFound("User was not found");

        var code = await _codeManager.FindAsync(user, context.Code, cancellationToken);
        if (code is null) return Results.BadRequest("Invalid code");
        
        var codeResult = await _codeManager.ConsumeAsync(code, cancellationToken);
        if (!codeResult.Succeeded) return codeResult;

        var method = code.Sender switch
        {
            SenderType.Email => VerificationMethod.EmailOtp,
            SenderType.Sms => VerificationMethod.SmsOtp,
            _ => throw new NotSupportedException("Unknown sender")
        };
        
        var requestEntity = new VerificationRequestEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Action = context.Action,
            Purpose = context.Purpose,
            Method = method,
            Status = VerificationStatus.Approved,
            ApprovedAt = DateTimeOffset.UtcNow,
            ExpiredAt = DateTimeOffset.UtcNow.Add(_configuration.Timestamp)
        };

        var verificationResult = await _verificationManager.CreateAsync(requestEntity, cancellationToken);
        if (!verificationResult.Succeeded) return verificationResult;

        var response = new VerificationResponse { VerificationId = requestEntity.Id };
        return Results.Ok(response);
    }
}
using System.Text.Json.Serialization;
using eSecurity.Core.Security.Authentication.TwoFactor;
using eSecurity.Idp.Security.Authentication.TwoFactor;
using eSecurity.Idp.Security.Authorization.Verification;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.TwoFactor;

public record EnableTwoFactorCommand : IRequest<Result>
{
    [JsonPropertyName("verification_id")]
    public required Guid VerificationId { get; set; }
}

public class EnableTwoFactorCommandHandler(
    ICurrentUserAccessor currentUserAccessor,
    ITwoFactorManager twoFactorManager,
    IVerificationQueryService verificationQueryService,
    IVerificationCommandService verificationCommandService) : IRequestHandler<EnableTwoFactorCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly ITwoFactorManager _twoFactorManager = twoFactorManager;
    private readonly IVerificationQueryService _verificationQueryService = verificationQueryService;
    private readonly IVerificationCommandService _verificationCommandService = verificationCommandService;

    public async Task<Result> Handle(EnableTwoFactorCommand request, CancellationToken cancellationToken)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        if (await _twoFactorManager.IsEnabledAsync(user, cancellationToken))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "2FA already enabled."
            });
        }
        
        var verification = await _verificationQueryService.GetByIdAsync(user.Id, 
            request.VerificationId, cancellationToken);
        
        if (verification is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid verification request"
            });
        }

        var verificationResult = await _verificationCommandService.ConsumeAsync(verification, cancellationToken);
        if (!verificationResult.Succeeded) return verificationResult;

        var authenticatorResult = await _twoFactorManager.SubscribeAsync(user,
            TwoFactorMethod.AuthenticatorApp, true, cancellationToken);

        if (!authenticatorResult.Succeeded) return authenticatorResult;

        var recoveryCodesResult = await _twoFactorManager.SubscribeAsync(user,
            TwoFactorMethod.RecoveryCode, cancellationToken: cancellationToken);

        return recoveryCodesResult;
    }
}
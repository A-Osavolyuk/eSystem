using System.Text.Json.Serialization;
using eSecurity.Idp.Security.Authentication.TwoFactor;
using eSecurity.Idp.Security.Authorization.Verification;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.TwoFactor;

public record DisableTwoFactorCommand : IRequest<Result>
{
    [JsonPropertyName("verification_id")]
    public Guid VerificationId { get; set; }
}

public class DisableTwoFactorCommandHandler(
    ICurrentUserAccessor currentUserAccessor,
    ITwoFactorManager twoFactorManager,
    IVerificationQueryService verificationQueryService,
    IVerificationCommandService verificationCommandService) : IRequestHandler<DisableTwoFactorCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly ITwoFactorManager _twoFactorManager = twoFactorManager;
    private readonly IVerificationQueryService _verificationQueryService = verificationQueryService;
    private readonly IVerificationCommandService _verificationCommandService = verificationCommandService;

    public async Task<Result> Handle(DisableTwoFactorCommand request, CancellationToken cancellationToken)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        if (await _twoFactorManager.IsEnabledAsync(user, cancellationToken))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "2FA already disabled."
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
        
        var methodResult = await _twoFactorManager.UnsubscribeAsync(user, cancellationToken);
        return methodResult;
    }
}
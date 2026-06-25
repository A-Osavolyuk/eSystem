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
    public Guid VerificationId { get; set; }
}

public class EnableTwoFactorCommandHandler(
    ICurrentUserAccessor currentUserAccessor,
    IVerificationQueryService verificationQueryService,
    ITwoFactorQueryService twoFactorQueryService,
    ITwoFactorCommandService twoFactorCommandService,
    IVerificationCommandService verificationCommandService) : IRequestHandler<EnableTwoFactorCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IVerificationQueryService _verificationQueryService = verificationQueryService;
    private readonly ITwoFactorQueryService _twoFactorQueryService = twoFactorQueryService;
    private readonly ITwoFactorCommandService _twoFactorCommandService = twoFactorCommandService;
    private readonly IVerificationCommandService _verificationCommandService = verificationCommandService;

    public async Task<Result> Handle(EnableTwoFactorCommand request, CancellationToken cancellationToken)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        var twoFactorMethods = await _twoFactorQueryService.ListByUserAsync(user.Id, cancellationToken);
        if (twoFactorMethods.Count > 0)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "2FA already enabled."
            });
        }
        
        var verification = await _verificationQueryService.GetByIdAsync(user.Id, request.VerificationId, cancellationToken);
        if (verification is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid verification request"
            });
        }

        var verificationResult = await _verificationCommandService.ConsumeAsync(verification, cancellationToken);
        if (!verificationResult.Succeeded) 
            return verificationResult;

        var addAuthenticatorAppTwoFactorMethodResult = await _twoFactorCommandService.AddMethodAsync(user.Id, 
            TwoFactorMethod.AuthenticatorApp, cancellationToken);

        if (!addAuthenticatorAppTwoFactorMethodResult.Succeeded) 
            return addAuthenticatorAppTwoFactorMethodResult;

        return await _twoFactorCommandService.AddMethodAsync(user.Id,
            TwoFactorMethod.RecoveryCode, cancellationToken);
    }
}
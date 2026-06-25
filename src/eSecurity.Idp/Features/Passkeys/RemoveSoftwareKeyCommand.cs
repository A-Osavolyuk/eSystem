using System.Text.Json.Serialization;
using eSecurity.Core.Security.Authentication.TwoFactor;
using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Security.Authentication.Password;
using eSecurity.Idp.Security.Authentication.TwoFactor;
using eSecurity.Idp.Security.Authorization.Verification;
using eSecurity.Idp.Security.Credentials.PublicKey;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.Options;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Passkeys;

public record RemoveSoftwareKeyCommand : IRequest<Result>
{
    [JsonPropertyName("verification_id")]
    public Guid VerificationId { get; set; }
    
    [JsonPropertyName("passkey_id")]
    public Guid PasskeyId { get; set; }
}

public class RemoveSoftwareKeyCommandHandler(
    ISoftwareKeyManager softwareKeyManager,
    IPasswordManager passwordManager,
    ITwoFactorManager twoFactorManager,
    ICurrentUserAccessor currentUserAccessor,
    IEmailQueryService emailQueryService,
    IVerificationQueryService verificationQueryService,
    IVerificationCommandService verificationCommandService,
    IOptions<SignInOptions> options) : IRequestHandler<RemoveSoftwareKeyCommand, Result>
{
    private readonly ISoftwareKeyManager _softwareKeyManager = softwareKeyManager;
    private readonly IPasswordManager _passwordManager = passwordManager;
    private readonly ITwoFactorManager _twoFactorManager = twoFactorManager;
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IEmailQueryService _emailQueryService = emailQueryService;
    private readonly IVerificationQueryService _verificationQueryService = verificationQueryService;
    private readonly IVerificationCommandService _verificationCommandService = verificationCommandService;
    private readonly SignInOptions _options = options.Value;

    public async Task<Result> Handle(RemoveSoftwareKeyCommand request, CancellationToken cancellationToken)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        var passkey = await _softwareKeyManager.FindByIdAsync(request.PasskeyId, cancellationToken);
        if (passkey is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "Passkey not found."
            });
        }

        var primaryEmail = await _emailQueryService.GetByTypeAsync(user.Id, EmailType.Primary, cancellationToken);
        if ((primaryEmail is null && _options.RequireConfirmedEmail) || 
            !await _passwordManager.HasAsync(user, cancellationToken))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "You need to enable another authentication method first."
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

        var passkeys = await _softwareKeyManager.GetAllAsync(user, cancellationToken);
        if (passkeys.Count == 1)
        {
            if (await _twoFactorManager.HasMethodAsync(user, TwoFactorMethod.Passkey, cancellationToken))
            {
                var method = await _twoFactorManager.GetAsync(user, TwoFactorMethod.Passkey, cancellationToken);
                if (method is null)
                {
                    return Results.ClientError(ClientErrorCode.NotFound, new Error
                    {
                        Code = ErrorCode.NotFound,
                        Description = "Method not found"
                    });
                }

                if (method.Preferred)
                {
                    var preferredResult = await _twoFactorManager.PreferAsync(user,
                        TwoFactorMethod.AuthenticatorApp, cancellationToken);
                    if (!preferredResult.Succeeded) return preferredResult;
                }

                var result = await _twoFactorManager.UnsubscribeAsync(method, cancellationToken);
                if (!result.Succeeded) return result;
            }
        }

        return await _softwareKeyManager.DeleteAsync(passkey, cancellationToken);
    }
}
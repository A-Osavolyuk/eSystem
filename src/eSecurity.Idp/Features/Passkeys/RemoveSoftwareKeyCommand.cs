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
    ISoftwareKeyQueryService softwareKeyQueryService,
    ISoftwareKeyCommandService softwareKeyCommandService,
    IPasswordManager passwordManager,
    ICurrentUserAccessor currentUserAccessor,
    IEmailQueryService emailQueryService,
    IVerificationQueryService verificationQueryService,
    IVerificationCommandService verificationCommandService,
    ITwoFactorQueryService twoFactorQueryService,
    ITwoFactorCommandService twoFactorCommandService,
    IOptions<SignInOptions> options) : IRequestHandler<RemoveSoftwareKeyCommand, Result>
{
    private readonly ISoftwareKeyQueryService _softwareKeyQueryService = softwareKeyQueryService;
    private readonly ISoftwareKeyCommandService _softwareKeyCommandService = softwareKeyCommandService;
    private readonly IPasswordManager _passwordManager = passwordManager;
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IEmailQueryService _emailQueryService = emailQueryService;
    private readonly IVerificationQueryService _verificationQueryService = verificationQueryService;
    private readonly IVerificationCommandService _verificationCommandService = verificationCommandService;
    private readonly ITwoFactorQueryService _twoFactorQueryService = twoFactorQueryService;
    private readonly ITwoFactorCommandService _twoFactorCommandService = twoFactorCommandService;
    private readonly SignInOptions _options = options.Value;

    public async Task<Result> Handle(RemoveSoftwareKeyCommand request, CancellationToken cancellationToken)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        var passkey = await _softwareKeyQueryService.GetByIdAsync(request.PasskeyId, cancellationToken);
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
        if (!verificationResult.Succeeded) 
            return verificationResult;

        var softwareKeys = await _softwareKeyQueryService.ListByUserAsync(user.Id, cancellationToken);
        if (softwareKeys.Count == 1)
        {
            var twoFactorMethod = await _twoFactorQueryService.GetByMethodAsync(user.Id, 
                TwoFactorMethod.SoftwareKey, cancellationToken);
            
            if (twoFactorMethod is not null && twoFactorMethod.Preferred)
            {
                var removeResult = await _twoFactorCommandService.RemoveMethodAsync(user.Id, 
                    twoFactorMethod.Id, cancellationToken);

                if (!removeResult.Succeeded)
                    return removeResult;
            }
        }

        return await _softwareKeyCommandService.DeleteAsync(passkey, cancellationToken);
    }
}
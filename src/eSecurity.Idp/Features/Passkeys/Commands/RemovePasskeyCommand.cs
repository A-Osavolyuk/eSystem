using eSecurity.Idp.Security.Authentication.Password;
using eSecurity.Idp.Security.Authentication.TwoFactor;
using eSecurity.Idp.Security.Authorization.Verification;
using eSecurity.Idp.Security.Credentials.PublicKey;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.Options;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.Core.Requests;
using eSecurity.Core.Security.Authentication.TwoFactor;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Core.Security.Identity;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Idp.Features.Passkeys.Commands;

public record RemovePasskeyCommand(RemovePasskeyRequest Request) : IRequest<Result>;

public class RemovePasskeyCommandHandler(
    IPasskeyManager passkeyManager,
    IPasswordManager passwordManager,
    IVerificationManager verificationManager,
    ITwoFactorManager twoFactorManager,
    ICurrentUserAccessor currentUserAccessor,
    IEmailQueryService emailQueryService,
    IOptions<SignInOptions> options) : IRequestHandler<RemovePasskeyCommand, Result>
{
    private readonly IPasskeyManager _passkeyManager = passkeyManager;
    private readonly IPasswordManager _passwordManager = passwordManager;
    private readonly IVerificationManager _verificationManager = verificationManager;
    private readonly ITwoFactorManager _twoFactorManager = twoFactorManager;
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IEmailQueryService _emailQueryService = emailQueryService;
    private readonly SignInOptions _options = options.Value;

    public async Task<Result> Handle(RemovePasskeyCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _currentUserAccessor.GetCurrentUserAsync(cancellationToken);
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

        var passkey = await _passkeyManager.FindByIdAsync(request.Request.PasskeyId, cancellationToken);
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

        var verification = await _verificationManager.FindByIdAsync(request.Request.VerificationId, cancellationToken);
        if (verification?.Status is not VerificationStatus.Approved)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "Unverified request."
            });
        }

        var verificationResult = await _verificationManager.ConsumeAsync(verification, cancellationToken);
        if (!verificationResult.Succeeded) return verificationResult;

        var passkeys = await _passkeyManager.GetAllAsync(user, cancellationToken);
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

        return await _passkeyManager.DeleteAsync(passkey, cancellationToken);
    }
}
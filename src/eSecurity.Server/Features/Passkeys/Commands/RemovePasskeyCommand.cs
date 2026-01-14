using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authentication.TwoFactor;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Security.Authentication.Password;
using eSecurity.Server.Security.Authentication.TwoFactor;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Credentials.PublicKey;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.Options;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Features.Passkeys.Commands;

public record RemovePasskeyCommand(RemovePasskeyRequest Request) : IRequest<Result>;

public class RemovePasskeyCommandHandler(
    IPasskeyManager passkeyManager,
    IPasswordManager passwordManager,
    IUserManager userManager,
    IEmailManager emailManager,
    IVerificationManager verificationManager,
    ITwoFactorManager twoFactorManager,
    IOptions<SignInOptions> options) : IRequestHandler<RemovePasskeyCommand, Result>
{
    private readonly IPasskeyManager _passkeyManager = passkeyManager;
    private readonly IPasswordManager _passwordManager = passwordManager;
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly IVerificationManager _verificationManager = verificationManager;
    private readonly ITwoFactorManager _twoFactorManager = twoFactorManager;
    private readonly SignInOptions _options = options.Value;

    public async Task<Result> Handle(RemovePasskeyCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        var passkey = await _passkeyManager.FindByIdAsync(request.Request.PasskeyId, cancellationToken);
        if (passkey is null) return Results.NotFound("Passkey not found.");

        if ((!await _emailManager.HasAsync(user, EmailType.Primary, cancellationToken) && 
             _options.RequireConfirmedEmail) || !await _passwordManager.HasAsync(user, cancellationToken))
            return Results.BadRequest("You need to enable another authentication method first.");

        var verificationResult = await _verificationManager.VerifyAsync(user,
            PurposeType.Passkey, ActionType.Remove, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        var passkeys = await _passkeyManager.GetAllAsync(user, cancellationToken);
        if (passkeys.Count == 1)
        {
            if (await _twoFactorManager.HasMethodAsync(user, TwoFactorMethod.Passkey, cancellationToken))
            {
                var method = await _twoFactorManager.GetAsync(user, TwoFactorMethod.Passkey, cancellationToken);
                if (method is null) return Results.NotFound("Method not found");

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
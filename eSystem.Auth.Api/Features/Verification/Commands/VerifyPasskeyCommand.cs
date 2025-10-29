using eSystem.Auth.Api.Security.Authorization.Access;
using eSystem.Auth.Api.Security.Credentials.PublicKey;
using eSystem.Auth.Api.Security.Credentials.PublicKey.Credentials;
using eSystem.Auth.Api.Security.Identity.User;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Security.Credentials.Constants;

namespace eSystem.Auth.Api.Features.Verification.Commands;

public record VerifyPasskeyCommand(VerifyPasskeyRequest Request) : IRequest<Result>;

public class VerifyPasskeyCommandHandler(
    IUserManager userManager,
    IPasskeyManager passkeyManager,
    IHttpContextAccessor httpContextAccessor,
    IVerificationManager verificationManager) : IRequestHandler<VerifyPasskeyCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IPasskeyManager passkeyManager = passkeyManager;
    private readonly IVerificationManager verificationManager = verificationManager;
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(VerifyPasskeyCommand request, CancellationToken cancellationToken)
    {
        var credential = request.Request.Credential;
        var credentialId = CredentialUtils.ToBase64String(credential.Id);

        var passkey = await passkeyManager.FindByCredentialIdAsync(credentialId, cancellationToken);
        if (passkey is null) return Results.BadRequest("Invalid credential");

        var user = await userManager.FindByIdAsync(passkey.Device.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {passkey.Device.UserId}.");

        var savedChallenge = httpContext.Session.GetString(ChallengeSessionKeys.Assertion);
        if (string.IsNullOrEmpty(savedChallenge)) return Results.BadRequest("Invalid challenge");

        var verificationResult = await passkeyManager.VerifyAsync(passkey, credential, savedChallenge, cancellationToken);
        if (!verificationResult.Succeeded) return verificationResult;

        var purpose = request.Request.Purpose;
        var action = request.Request.Action;
        var result = await verificationManager.CreateAsync(user, purpose, action, cancellationToken);
        return result;
    }
}
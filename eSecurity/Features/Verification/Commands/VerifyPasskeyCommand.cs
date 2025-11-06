using eSecurity.Security.Authorization.Access;
using eSecurity.Security.Credentials.PublicKey;
using eSecurity.Security.Credentials.PublicKey.Credentials;
using eSecurity.Security.Identity.User;
using eSystem.Core.Security.Authorization.Access;
using eSystem.Core.Security.Credentials.Constants;
using eSystem.Core.Security.Credentials.PublicKey;

namespace eSecurity.Features.Verification.Commands;

public record VerifyPasskeyCommand() : IRequest<Result>
{
    public required Guid UserId { get; set; }
    public required PurposeType Purpose { get; set; }
    public required ActionType Action { get; set; }
    public required PublicKeyCredential Credential { get; set; }
}

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
        var credential = request.Credential;
        var credentialId = CredentialUtils.ToBase64String(credential.Id);

        var passkey = await passkeyManager.FindByCredentialIdAsync(credentialId, cancellationToken);
        if (passkey is null) return Results.BadRequest("Invalid credential");

        var user = await userManager.FindByIdAsync(passkey.Device.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {passkey.Device.UserId}.");

        var savedChallenge = httpContext.Session.GetString(ChallengeSessionKeys.Assertion);
        if (string.IsNullOrEmpty(savedChallenge)) return Results.BadRequest("Invalid challenge");

        var verificationResult = await passkeyManager.VerifyAsync(passkey, credential, savedChallenge, cancellationToken);
        if (!verificationResult.Succeeded) return verificationResult;

        var purpose = request.Purpose;
        var action = request.Action;
        var result = await verificationManager.CreateAsync(user, purpose, action, cancellationToken);
        return result;
    }
}
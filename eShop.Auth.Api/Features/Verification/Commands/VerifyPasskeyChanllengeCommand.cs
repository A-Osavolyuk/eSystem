using eShop.Domain.Common.Security.Constants;
using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Features.Verification.Commands;

public record VerifyPasskeyChallengeCommand(VerifyPasskeyChallengeRequest Request) : IRequest<Result>;

public class VerifyPasskeyChallengeCommandHandler(
    IUserManager userManager,
    IPasskeyManager passkeyManager,
    IHttpContextAccessor httpContextAccessor,
    IVerificationManager verificationManager) : IRequestHandler<VerifyPasskeyChallengeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IPasskeyManager passkeyManager = passkeyManager;
    private readonly IVerificationManager verificationManager = verificationManager;
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(VerifyPasskeyChallengeCommand request, CancellationToken cancellationToken)
    {
        var credential = request.Request.Credential;
        var credentialId = CredentialUtils.ToBase64String(credential.Id);

        var passkey = await passkeyManager.FindByCredentialIdAsync(credentialId, cancellationToken);
        if (passkey is null) return Results.BadRequest("Invalid credential");

        var user = await userManager.FindByIdAsync(passkey.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {passkey.UserId}.");

        var savedChallenge = httpContext.Session.GetString(ChallengeSessionKeys.Verification);
        if (string.IsNullOrEmpty(savedChallenge)) return Results.BadRequest("Invalid challenge");

        var verificationResult = await passkeyManager.VerifyAsync(passkey, credential, savedChallenge, cancellationToken);
        if (!verificationResult.Succeeded) return verificationResult;

        var purpose = request.Request.Purpose;
        var action = request.Request.Action;
        var result = await verificationManager.CreateAsync(user, purpose, action, cancellationToken);
        return result;
    }
}
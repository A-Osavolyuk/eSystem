using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Credentials.PublicKey.Constants;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Credentials.PublicKey;
using eSecurity.Server.Security.Credentials.PublicKey.Credentials;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Features.Verification.Commands;

public record VerifyPasskeyCommand(VerifyPasskeyRequest Request) : IRequest<Result>;

public class VerifyPasskeyCommandHandler(
    IUserManager userManager,
    IPasskeyManager passkeyManager,
    IHttpContextAccessor httpContextAccessor,
    IVerificationManager verificationManager) : IRequestHandler<VerifyPasskeyCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPasskeyManager _passkeyManager = passkeyManager;
    private readonly IVerificationManager _verificationManager = verificationManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(VerifyPasskeyCommand request, CancellationToken cancellationToken)
    {
        var credential = request.Request.Credential;
        var credentialId = CredentialUtils.ToBase64String(credential.Id);

        var passkey = await _passkeyManager.FindByCredentialIdAsync(credentialId, cancellationToken);
        if (passkey is null)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.InvalidCredentials,
                Description = "Invalid credential"
            });
        }

        var user = await _userManager.FindByIdAsync(passkey.Device.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        var savedChallenge = _httpContext.Session.GetString(ChallengeSessionKeys.Assertion);
        if (string.IsNullOrEmpty(savedChallenge))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.InvalidChallenge,
                Description = "Invalid challenge"
            });
        }

        var verificationResult = await _passkeyManager.VerifyAsync(passkey, credential, savedChallenge, cancellationToken);
        if (!verificationResult.Succeeded) return verificationResult;

        var purpose = request.Request.Purpose;
        var action = request.Request.Action;
        var result = await _verificationManager.CreateAsync(user, purpose, action, cancellationToken);
        return result;
    }
}
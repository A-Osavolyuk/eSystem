using eShop.Domain.Common.Security.Constants;
using eShop.Domain.Requests.Auth;
using eShop.Domain.Responses.Auth;

namespace eShop.Auth.Api.Features.Passkeys.Commands;

public record PasskeySignInCommand(PasskeySignInRequest Request) : IRequest<Result>;

public class PasskeySignInCommandHandler(
    IUserManager userManager,
    IPasskeyManager passkeyManager,
    ILoginManager loginManager,
    IAuthorizationManager authorizationManager,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<PasskeySignInCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IPasskeyManager passkeyManager = passkeyManager;
    private readonly ILoginManager loginManager = loginManager;
    private readonly IAuthorizationManager authorizationManager = authorizationManager;
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(PasskeySignInCommand request,
        CancellationToken cancellationToken)
    {
        var credential = request.Request.Credential;
        var credentialId = CredentialUtils.ToBase64String(credential.Id);

        var passkey = await passkeyManager.FindByCredentialIdAsync(credentialId, cancellationToken);
        if (passkey is null) return Results.BadRequest("Invalid credential");

        var user = await userManager.FindByIdAsync(passkey.Device.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {passkey.Device.UserId}.");

        var savedChallenge = httpContext.Session.GetString(ChallengeSessionKeys.Assertion);
        if (string.IsNullOrEmpty(savedChallenge)) return Results.BadRequest("Invalid challenge");

        var result = await passkeyManager.VerifyAsync(passkey, credential, savedChallenge, cancellationToken);
        if (!result.Succeeded) return result;

        if (user.LockoutState.Enabled)
        {
            var lockoutResponse = new PasskeySignInResponse()
            {
                UserId = user.Id,
                IsLockedOut = user.LockoutState.Enabled,
            };

            return Results.BadRequest("Account is locked out", lockoutResponse);
        }

        var userAgent = httpContext.GetUserAgent()!;
        var ipAddress = httpContext.GetIpV4()!;
        var device = user.GetDevice(userAgent, ipAddress);
        if (device is null) return Results.NotFound($"Invalid device.");

        await loginManager.CreateAsync(device, LoginType.Passkey, cancellationToken);
        await authorizationManager.CreateAsync(device, cancellationToken);

        var response = new PasskeySignInResponse()
        {
            UserId = user.Id,
        };

        return Result.Success(response);
    }
}
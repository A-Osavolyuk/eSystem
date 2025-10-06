using eShop.Auth.Api.Types;
using eShop.Domain.Common.Security.Constants;
using eShop.Domain.Requests.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Passkeys.Commands;

public record VerifyPasskeySignInCommand(VerifyPasskeySignInRequest Request) : IRequest<Result>;

public class VerifyPasskeySignInCommandHandler(
    IUserManager userManager,
    IPasskeyManager passkeyManager,
    ILockoutManager lockoutManager,
    ILoginManager loginManager,
    IAuthorizationManager authorizationManager,
    IDeviceManager deviceManager,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<VerifyPasskeySignInCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IPasskeyManager passkeyManager = passkeyManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly ILoginManager loginManager = loginManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly IAuthorizationManager authorizationManager = authorizationManager;
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(VerifyPasskeySignInCommand request,
        CancellationToken cancellationToken)
    {
        var credential = request.Request.Credential;
        var credentialId = CredentialUtils.ToBase64String(credential.Id);

        var passkey = await passkeyManager.FindByCredentialIdAsync(credentialId, cancellationToken);
        if (passkey is null) return Results.BadRequest("Invalid credential");

        var user = await userManager.FindByIdAsync(passkey.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {passkey.UserId}.");

        var savedChallenge = httpContext.Session.GetString("webauthn_assertion_challenge");
        if (string.IsNullOrEmpty(savedChallenge)) return Results.BadRequest("Invalid challenge");

        var result = await passkeyManager.VerifyAsync(passkey, credential, savedChallenge, cancellationToken);
        if (!result.Succeeded) return result;

        var lockoutState = await lockoutManager.FindAsync(user, cancellationToken);

        if (lockoutState.Enabled)
        {
            var lockoutResponse = new VerifyPasskeySignInResponse()
            {
                UserId = user.Id,
                IsLockedOut = lockoutState.Enabled,
            };

            return Results.BadRequest("Account is locked out", lockoutResponse);
        }

        var userAgent = httpContext.GetUserAgent()!;
        var ipV4 = httpContext.GetIpV4()!;

        var device = await deviceManager.FindAsync(user, userAgent, ipV4, cancellationToken);
        if (device is null) return Results.NotFound($"Invalid device.");

        await loginManager.CreateAsync(device, LoginType.Passkey, cancellationToken);
        await authorizationManager.CreateAsync(device, cancellationToken);

        var response = new VerifyPasskeySignInResponse()
        {
            UserId = user.Id,
        };

        return Result.Success(response);
    }
}
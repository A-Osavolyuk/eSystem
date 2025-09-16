using eShop.Auth.Api.Types;
using eShop.Domain.Common.Security.Constants;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Passkeys.Commands;

public record VerifyPasskeySignInCommand(VerifyPasskeySignInRequest Request, HttpContext HttpContext) : IRequest<Result>;

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
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

    public async Task<Result> Handle(VerifyPasskeySignInCommand request,
        CancellationToken cancellationToken)
    {
        var credentialId = CredentialUtils.ToBase64String(request.Request.Credential.Id);

        var passkey = await passkeyManager.FindByCredentialIdAsync(credentialId, cancellationToken);
        if (passkey is null) return Results.BadRequest("Invalid credential");

        var user = await userManager.FindByIdAsync(passkey.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {passkey.UserId}.");

        var authenticatorAssertionResponse = request.Request.Credential.Response;
        var clientData = ClientData.Parse(authenticatorAssertionResponse.ClientDataJson);

        if (clientData is null) return Results.BadRequest("Invalid client data");
        if (clientData.Type != ClientDataTypes.Get) return Results.BadRequest("Invalid type");

        var base64Challenge = CredentialUtils.ToBase64String(clientData.Challenge);
        var savedChallenge = request.HttpContext.Session.GetString("webauthn_assertion_challenge");

        if (savedChallenge != base64Challenge) return Results.BadRequest("Challenge mismatch");

        var valid = CredentialUtils.VerifySignature(authenticatorAssertionResponse, passkey.PublicKey);
        if (!valid) return Results.BadRequest("Invalid client data");

        var signCount = CredentialUtils.ParseSignCount(authenticatorAssertionResponse.AuthenticatorData);

        passkey.SignCount = signCount;
        passkey.UpdateDate = DateTimeOffset.UtcNow;

        var result = await passkeyManager.UpdateAsync(passkey, cancellationToken);
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
        
        var userAgent = httpContextAccessor.HttpContext?.GetUserAgent()!;
        var ipV4 = httpContextAccessor.HttpContext?.GetIpV4()!;

        var device = await deviceManager.FindAsync(user, userAgent, ipV4, cancellationToken);
        if (device is null) return Results.NotFound($"Invalid device.");
        
        await loginManager.CreateAsync(device, LoginType.Passkey, cancellationToken);
        await authorizationManager.CreateAsync(user, device, cancellationToken);
        
        var response = new VerifyPasskeySignInResponse()
        {
            UserId = user.Id,
        };

        return Result.Success(response);
    }
}
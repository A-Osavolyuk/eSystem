using System.Security.Cryptography;
using System.Text.Json;
using eShop.Auth.Api.Types;
using eShop.Domain.Constants;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Passkeys.Commands;

public record VerifyPasskeySignInCommand(VerifyPasskeySignInRequest Request, HttpContext HttpContext) : IRequest<Result>;

public class VerifyPasskeySignInCommandHandler(
    IUserManager userManager,
    IPasskeyManager passkeyManager,
    ITokenManager tokenManager,
    ILockoutManager lockoutManager) : IRequestHandler<VerifyPasskeySignInCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IPasskeyManager passkeyManager = passkeyManager;
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;

    public async Task<Result> Handle(VerifyPasskeySignInCommand request,
        CancellationToken cancellationToken)
    {
        var credentialIdBytes = CredentialUtils.Base64UrlDecode(request.Request.Credential.Id);
        var base64CredentialId = Convert.ToBase64String(credentialIdBytes);

        var passkey = await passkeyManager.FindByCredentialIdAsync(base64CredentialId, cancellationToken);
        if (passkey is null) return Results.BadRequest("Invalid credential");

        var user = await userManager.FindByIdAsync(passkey.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {passkey.UserId}.");

        var authenticationAssertionResponse = request.Request.Credential.Response;

        var clientDataJson = CredentialUtils.Base64UrlDecode(authenticationAssertionResponse.ClientDataJson);
        var clientData = JsonSerializer.Deserialize<ClientData>(clientDataJson);

        if (clientData is null) return Results.BadRequest("Invalid client data");
        if (clientData.Type != ClientDataTypes.Get) return Results.BadRequest("Invalid type");

        var challengeBytes = CredentialUtils.Base64UrlDecode(clientData.Challenge);
        var base64Challenge = Convert.ToBase64String(challengeBytes);
        var savedChallenge = request.HttpContext.Session.GetString("webauthn_assertion_challenge");

        if (savedChallenge != base64Challenge) return Results.BadRequest("Challenge mismatch");

        var authenticatorData = CredentialUtils.Base64UrlDecode(authenticationAssertionResponse.AuthenticatorData);
        var signature = CredentialUtils.Base64UrlDecode(authenticationAssertionResponse.Signature);

        using var sha256 = SHA256.Create();
        var clientDataHash = sha256.ComputeHash(clientDataJson);

        var signedData = authenticatorData.Concat(clientDataHash).ToArray();

        using var key = CredentialUtils.ImportCosePublicKey(passkey.PublicKey);

        var valid = key switch
        {
            ECDsa ecdsa => ecdsa.VerifyData(signedData, signature, HashAlgorithmName.SHA256),
            RSA rsa => rsa.VerifyData(signedData, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1),
            _ => throw new NotSupportedException("Unsupported key type")
        };

        if (!valid) return Results.BadRequest("Invalid client data");
        
        var signCount = CredentialUtils.ParseSignCount(authenticatorData);
        
        passkey.SignCount = signCount;
        passkey.UpdateDate = DateTimeOffset.UtcNow;
        
        var result = await passkeyManager.UpdateAsync(passkey, cancellationToken);
        if (!result.Succeeded) return result;
        
        var lockoutState = await lockoutManager.FindAsync(user, cancellationToken);

        if (lockoutState.Enabled)
        {
            var lockoutResponse = new VerifyPublicKeyCredentialRequestOptionsResponse()
            {
                UserId = user.Id,
                IsLockedOut = lockoutState.Enabled,
            };
            
            return Results.BadRequest("Account is locked out", lockoutResponse);
        }

        var accessToken = await tokenManager.GenerateAsync(user, TokenType.Access, cancellationToken);
        var refreshToken = await tokenManager.GenerateAsync(user, TokenType.Refresh, cancellationToken);

        var response = new VerifyPublicKeyCredentialRequestOptionsResponse()
        {
            UserId = user.Id,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        return Result.Success(response);
    }
}
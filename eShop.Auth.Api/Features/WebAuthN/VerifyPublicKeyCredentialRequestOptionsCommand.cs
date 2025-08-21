using System.Security.Cryptography;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.WebAuthN;

public record VerifyPublicKeyCredentialRequestOptionsCommand(
    VerifyPublicKeyCredentialRequestOptionsRequest Request,
    HttpContext HttpContext) : IRequest<Result>;

public class VerifyPublicKeyCredentialRequestOptionsCommandHandler(
    IUserManager userManager,
    ICredentialManager credentialManager,
    ITokenManager tokenManager) : IRequestHandler<VerifyPublicKeyCredentialRequestOptionsCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ICredentialManager credentialManager = credentialManager;
    private readonly ITokenManager tokenManager = tokenManager;

    public async Task<Result> Handle(VerifyPublicKeyCredentialRequestOptionsCommand request,
        CancellationToken cancellationToken)
    {
        var credentialIdBytes = CredentialUtils.Base64UrlDecode(request.Request.Credential.Id);
        var base64CredentialId = Convert.ToBase64String(credentialIdBytes);

        var credential = await credentialManager.FindAsync(base64CredentialId, cancellationToken);
        if (credential is null) return Results.BadRequest("Invalid credential");

        var user = await userManager.FindByIdAsync(credential.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {credential.UserId}.");

        var authenticationAssertionResponse = request.Request.Credential.Response;

        var clientDataJson = CredentialUtils.Base64UrlDecode(authenticationAssertionResponse.ClientDataJson);
        var authenticatorData = CredentialUtils.Base64UrlDecode(authenticationAssertionResponse.AuthenticatorData);
        var signature = CredentialUtils.Base64UrlDecode(authenticationAssertionResponse.Signature);

        using var sha256 = SHA256.Create();
        var clientDataHash = sha256.ComputeHash(clientDataJson);

        var signedData = authenticatorData.Concat(clientDataHash).ToArray();

        using var key = CredentialUtils.ImportCosePublicKey(credential.PublicKey);

        var valid = key switch
        {
            ECDsa ecdsa => ecdsa.VerifyData(signedData, signature, HashAlgorithmName.SHA256),
            RSA rsa => rsa.VerifyData(signedData, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1),
            _ => throw new NotSupportedException("Unsupported key type")
        };

        if (!valid) return Results.BadRequest("Invalid client data");
        
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
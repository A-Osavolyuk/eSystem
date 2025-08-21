using System.Security.Cryptography;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.WebAuthN;

public record VerifyPublicKeyCredentialRequestOptionsCommand(
    VerifyPublicKeyCredentialRequestOptionsRequest Request,
    HttpContext HttpContext) : IRequest<Result>;

public class VerifyPublicKeyCredentialRequestOptionsCommandHandler(
    IUserManager userManager,
    ICredentialManager credentialManager) : IRequestHandler<VerifyPublicKeyCredentialRequestOptionsCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ICredentialManager credentialManager = credentialManager;

    public async Task<Result> Handle(VerifyPublicKeyCredentialRequestOptionsCommand request,
        CancellationToken cancellationToken)
    {
        var credentialIdBytes = CredentialUtils.Base64UrlDecode(request.Request.Credential.Id);
        var base64CredentialId = Convert.ToBase64String(credentialIdBytes);

        var credential = await credentialManager.FindAsync(base64CredentialId, cancellationToken);
        if (credential is null) return Results.BadRequest("Invalid credential");

        var user = await userManager.FindByIdAsync(credential.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {credential.UserId}.");

        var response = request.Request.Credential.Response;

        var clientDataJson = CredentialUtils.Base64UrlDecode(response.ClientDataJson);
        var authenticatorData = CredentialUtils.Base64UrlDecode(response.AuthenticatorData);
        var signature = CredentialUtils.Base64UrlDecode(response.Signature);

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

        return Result.Success();
    }
}
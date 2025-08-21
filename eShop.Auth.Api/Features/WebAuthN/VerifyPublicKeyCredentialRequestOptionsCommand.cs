using System.Security.Cryptography;
using System.Text.Json;
using eShop.Auth.Api.Types;
using eShop.Domain.Requests.API.Auth;
using PeterO.Cbor;

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

    public async Task<Result> Handle(VerifyPublicKeyCredentialRequestOptionsCommand request, CancellationToken cancellationToken)
    {
        var credential = await credentialManager.FindAsync(request.Request.Credential.Id, cancellationToken);
        if (credential is null) return Results.BadRequest("Invalid credential");
        
        var user = await userManager.FindByIdAsync(credential.UserId, cancellationToken);
        if(user is null) return Results.NotFound($"Cannot find user with ID {credential.UserId}.");

        var response = request.Request.Credential.Response;
        
        var clientDataJson = Base64UrlDecode(response.ClientDataJson);
        var authenticatorData = Base64UrlDecode(response.AuthenticatorData);
        var signature = Base64UrlDecode(response.Signature);
        
        using var sha256 = SHA256.Create();
        var clientDataHash = sha256.ComputeHash(clientDataJson);
        
        var signedData = authenticatorData.Concat(clientDataHash).ToArray();
        
        using var ecdsa = ImportCosePublicKey(credential.PublicKey);
        var valid = ecdsa.VerifyData(signedData, signature, HashAlgorithmName.SHA256);
        
        if(!valid) return Results.BadRequest("Invalid client data");
        
        return Result.Success();
    }
    
    public static byte[] Base64UrlDecode(string base64Url)
    {
        var padded = base64Url
            .Replace('-', '+')
            .Replace('_', '/');
        
        switch (padded.Length % 4)
        {
            case 2: padded += "=="; break;
            case 3: padded += "="; break;
        }

        return Convert.FromBase64String(padded);
    }
    
    public static ECDsa ImportCosePublicKey(byte[] coseKey)
    {
        var obj = CBORObject.DecodeFromBytes(coseKey);
        var x = obj[-2].GetByteString();
        var y = obj[-3].GetByteString();

        var ecdsa = ECDsa.Create(new ECParameters
        {
            Curve = ECCurve.NamedCurves.nistP256,
            Q = new ECPoint { X = x, Y = y }
        });

        return ecdsa;
    }
}
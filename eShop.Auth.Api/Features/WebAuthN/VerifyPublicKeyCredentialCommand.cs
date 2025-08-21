using System.Security.Cryptography;
using System.Text.Json;
using eShop.Auth.Api.Types;
using eShop.Domain.Requests.API.Auth;
using PeterO.Cbor;

namespace eShop.Auth.Api.Features.Security.Commands;

public record VerifyPublicKeyCredentialCommand(
    VerifyPublicKeyCredentialRequest Request, HttpContext HttpContext) : IRequest<Result>;

public class VerifyPublicKeyCredentialCommandHandler(
    IUserManager userManager,
    ICredentialManager credentialManager) : IRequestHandler<VerifyPublicKeyCredentialCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ICredentialManager credentialManager = credentialManager;

    public async Task<Result> Handle(VerifyPublicKeyCredentialCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var response = request.Request.Response;
        var clientDataBytes = Base64UrlDecode(response.Response.ClientDataJson);
        var clientDataJson = Encoding.UTF8.GetString(clientDataBytes);
        var clientData = JsonSerializer.Deserialize<ClientData>(clientDataJson);

        if (clientData is null) return Results.BadRequest("Invalid client data");
        if (clientData.Type != "webauthn.create") return Results.BadRequest("Invalid type");

        var challengeBytes = Base64UrlDecode(clientData.Challenge);
        var base64Challenge = Convert.ToBase64String(challengeBytes);
        var savedChallenge = request.HttpContext.Session.GetString("webauthn_attestation_challenge");

        if (savedChallenge != base64Challenge) return Results.BadRequest("Challenge mismatch");

        var attestationBytes = Base64UrlDecode(response.Response.AttestationObject);
        var attestationCbor = CBORObject.DecodeFromBytes(attestationBytes);
        var authDataBytes = attestationCbor["authData"].GetByteString();
        var authData = AuthenticationData.FromBytes(authDataBytes);
        
        var rpHash = SHA256.HashData("localhost"u8.ToArray());
        if (!authData.RpIdHash.SequenceEqual(rpHash)) return Results.BadRequest("Invalid RP ID");

        var userCredential = new UserCredentialEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Domain = clientData.Origin,
            CredentialId = Convert.ToBase64String(authData.CredentialId),
            PublicKey = authData.CredentialPublicKey,
            SignCount = authData.SignCount,
            CreateDate = DateTimeOffset.UtcNow
        };

        var result = await credentialManager.CreateAsync(userCredential, cancellationToken);
        return result;
    }

    private byte[] Base64UrlDecode(string base64Url)
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
}
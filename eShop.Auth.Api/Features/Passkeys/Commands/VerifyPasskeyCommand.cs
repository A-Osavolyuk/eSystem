using System.Security.Cryptography;
using eShop.Auth.Api.Types;
using eShop.Domain.Common.Security.Constants;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Passkeys.Commands;

public record VerifyPasskeyCommand(VerifyPasskeyRequest Request, HttpContext HttpContext) : IRequest<Result>;

public class VerifyPasskeyCommandHandler(
    IUserManager userManager,
    IPasskeyManager passkeyManager,
    IdentityOptions identityOptions) : IRequestHandler<VerifyPasskeyCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IPasskeyManager passkeyManager = passkeyManager;
    private readonly IdentityOptions identityOptions = identityOptions;

    public async Task<Result> Handle(VerifyPasskeyCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var response = request.Request.Response;
        var clientData = ClientData.Parse(response.Response.ClientDataJson);

        if (clientData is null) return Results.BadRequest("Invalid client data");
        if (clientData.Type != ClientDataTypes.Create) return Results.BadRequest("Invalid type");

        var base64Challenge = CredentialUtils.ToBase64String(clientData.Challenge);
        var savedChallenge = request.HttpContext.Session.GetString("webauthn_attestation_challenge");

        if (savedChallenge != base64Challenge) return Results.BadRequest("Challenge mismatch");
        
        var authData = AuthenticationData.Parse(response.Response.AttestationObject);
        
        var rpHash = SHA256.HashData(Encoding.UTF8.GetBytes(identityOptions.Credentials.Domain.ToArray()));
        if (!authData.RpIdHash.SequenceEqual(rpHash)) return Results.BadRequest("Invalid RP ID");

        var passkey = new UserPasskeyEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            DisplayName = request.Request.DisplayName,
            Domain = clientData.Origin,
            CredentialId = Convert.ToBase64String(authData.CredentialId),
            PublicKey = authData.CredentialPublicKey,
            SignCount = authData.SignCount,
            CreateDate = DateTimeOffset.UtcNow,
            Type = request.Request.Response.Type
        };

        var result = await passkeyManager.CreateAsync(passkey, cancellationToken);
        return result;
    }
}
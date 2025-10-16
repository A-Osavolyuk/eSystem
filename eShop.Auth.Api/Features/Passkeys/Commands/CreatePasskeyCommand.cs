using System.Security.Cryptography;
using eShop.Auth.Api.Constants;
using eShop.Auth.Api.Types;
using eShop.Domain.Common.Security.Constants;
using eShop.Domain.Requests.Auth;
using eShop.Domain.Responses.Auth;

namespace eShop.Auth.Api.Features.Passkeys.Commands;

public record CreatePasskeyCommand(CreatePasskeyRequest Request) : IRequest<Result>;

public class CreatePasskeyCommandHandler(
    IUserManager userManager,
    IPasskeyManager passkeyManager,
    ITwoFactorManager twoFactorManager,
    IHttpContextAccessor httpContextAccessor,
    IVerificationManager verificationManager,
    IdentityOptions identityOptions) : IRequestHandler<CreatePasskeyCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IPasskeyManager passkeyManager = passkeyManager;
    private readonly ITwoFactorManager twoFactorManager = twoFactorManager;
    private readonly IVerificationManager verificationManager = verificationManager;
    private readonly IdentityOptions identityOptions = identityOptions;
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(CreatePasskeyCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var verificationResult = await verificationManager.VerifyAsync(user,
            PurposeType.Passkey, ActionType.Create, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        var credentialResponse = request.Request.Response;
        var clientData = ClientData.Parse(credentialResponse.Response.ClientDataJson);

        if (clientData is null) return Results.BadRequest("Invalid client data");
        if (clientData.Type != ClientDataTypes.Create) return Results.BadRequest("Invalid type");

        var base64Challenge = CredentialUtils.ToBase64String(clientData.Challenge);
        var savedChallenge = httpContext.Session.GetString(ChallengeSessionKeys.Attestation);

        if (savedChallenge != base64Challenge) return Results.BadRequest("Challenge mismatch");

        var authData = AuthenticationData.Parse(credentialResponse.Response.AttestationObject);

        var source = Encoding.UTF8.GetBytes(identityOptions.Credentials.Domain.ToArray());
        var rpHash = SHA256.HashData(source);
        if (!authData.RpIdHash.SequenceEqual(rpHash)) return Results.BadRequest("Invalid RP ID");

        var passkey = new UserPasskeyEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            AuthenticatorId = new Guid(authData.AaGuid),
            DisplayName = request.Request.DisplayName,
            Domain = clientData.Origin,
            CredentialId = Convert.ToBase64String(authData.CredentialId),
            PublicKey = authData.CredentialPublicKey,
            SignCount = authData.SignCount,
            CreateDate = DateTimeOffset.UtcNow,
            Type = request.Request.Response.Type
        };

        var result = await passkeyManager.CreateAsync(passkey, cancellationToken);
        if (!result.Succeeded) return result;

        if (user.TwoFactorEnabled && !user.HasTwoFactor(TwoFactorMethod.Passkey))
        {
            var twoFactorResult = await twoFactorManager.SubscribeAsync(user,
                TwoFactorMethod.Passkey, cancellationToken: cancellationToken);

            if (!twoFactorResult.Succeeded) return twoFactorResult;
        }

        if (!user.HasVerificationMethod(VerificationMethod.Passkey))
        {
            var verificationMethodResult = await verificationManager.SubscribeAsync(user,
                VerificationMethod.Passkey, cancellationToken: cancellationToken);

            if (!verificationMethodResult.Succeeded) return verificationMethodResult;
        }

        var response = new CreatePasskeyResponse() { PasskeyId = passkey.Id };
        return Result.Success(response);
    }
}
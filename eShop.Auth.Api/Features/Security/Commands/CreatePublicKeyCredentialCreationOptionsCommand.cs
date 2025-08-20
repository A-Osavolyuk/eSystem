using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Types;
using OtpNet;

namespace eShop.Auth.Api.Features.Security.Commands;

public record CreatePublicKeyCredentialCreationOptionsCommand(
    CreatePublicKeyCredentialCreationOptionsRequest Request) : IRequest<Result>;

public class CreatePublicKeyCredentialCreationOptionsCommandHandler(
    IUserManager userManager) : IRequestHandler<CreatePublicKeyCredentialCreationOptionsCommand, Result>
{
    private readonly IUserManager userManager = userManager;

    public Task<Result> Handle(CreatePublicKeyCredentialCreationOptionsCommand request,
        CancellationToken cancellationToken)
    {
        var challenge = KeyGeneration.GenerateRandomKey(32);
        var userIdBytes = Guid.CreateVersion7().ToByteArray();

        var options = new PublicKeyCredentialCreationOptions()
        {
            Challenge = challenge,
            Attestation = Attestation.None,
            Timeout = 60000,
            User = new()
            {
                Id = userIdBytes,
                Name = "Username",
                DisplayName = "Username`s key"
            },
            AuthenticatorSelection = new()
            {
                AuthenticatorAttachment = AuthenticatorAttachment.Platform,
                UserVerification = UserVerification.Preferred,
                ResidentKey = ResidentKey.Preferred
            },
            PublicKeyCredentialParameters =
            [
                new() { Algorithm = Algorithm.Es256, Type = KeyType.PublicKey },
                new() { Algorithm = Algorithm.Rs256, Type = KeyType.PublicKey },
            ],
            ReplyingParty = new ReplyingParty()
            {
                Domain = "localhost",
                Name = "eAccount",
            }
        };

        var response = Result.Success(options);
        return Task.FromResult(response);
    }
}
using eSecurity.Data.Entities;
using eSystem.Core.Security.Credentials.Constants;
using eSystem.Core.Security.Credentials.PublicKey;

namespace eSecurity.Security.Credentials.PublicKey.Credentials;

public class CredentialFactory : ICredentialFactory
{
    public PublicKeyCredentialCreationOptions CreateCreationOptions(
        UserEntity user, 
        string displayName, 
        string challenge,
        string fingerprint, 
        CredentialOptions credentialOptions)
    {
        var identifier = $"{user.Id}_{fingerprint}";
        var identifierBytes = Encoding.UTF8.GetBytes(identifier);
        var identifierBase64 = Convert.ToBase64String(identifierBytes);
        
        return new PublicKeyCredentialCreationOptions()
        {
            Challenge = challenge,
            PublicKeyCredentialUser = new PublicKeyCredentialUser
            {
                Id = identifierBase64,
                Name = user.Username,
                DisplayName = displayName,
            },
            AuthenticatorSelection = new AuthenticatorSelection
            {
                AuthenticatorAttachment = AuthenticatorAttachments.Platform,
                UserVerification = UserVerifications.Required,
                ResidentKey = ResidentKeys.Preferred
            },
            PublicKeyCredentialParameters =
            [
                new PublicKeyCredentialParameter { Algorithm = Algorithms.Es256, Type = KeyType.PublicKey },
                new PublicKeyCredentialParameter { Algorithm = Algorithms.Rs256, Type = KeyType.PublicKey },
            ],
            ReplyingParty = new ReplyingParty()
            {
                Domain = credentialOptions.Domain,
                Name = credentialOptions.Server,
            },
            Attestation = Attestations.Direct,
            Timeout = credentialOptions.Timeout,
        };
    }

    public PublicKeyCredentialRequestOptions CreateRequestOptions(
        PasskeyEntity passkey, 
        string challenge,
        CredentialOptions credentialOptions)
    {
        var allowCredentials = new List<PublicKeyCredentialDescriptor>()
        {
            new()
            {
                Type = KeyType.PublicKey,
                Id = passkey.CredentialId,
                Transports = [CredentialTransports.Internal]
            }
        };

        return new PublicKeyCredentialRequestOptions()
        {
            Challenge = challenge,
            Timeout = credentialOptions.Timeout,
            Domain = credentialOptions.Domain,
            UserVerification = UserVerifications.Required,
            AllowCredentials = allowCredentials
        };
    }
}
using eShop.Domain.Common.Security.Constants;
using eShop.Domain.Common.Security.Credentials;
using OtpNet;

namespace eShop.Auth.Api.Utilities;

public static class CredentialGenerator
{
    public static string GenerateChallenge()
    {
        var challengeBytes = KeyGeneration.GenerateRandomKey(32);
        var challenge = Convert.ToBase64String(challengeBytes);
        return challenge;
    }
    
    public static PublicKeyCredentialCreationOptions CreateCreationOptions(
        UserEntity user, 
        string displayName, 
        string challenge,
        string fingerprint,
        CredentialOptions credentialOptions)
    {
        var identifier = $"{user.Id}_{fingerprint}";
        var identifierBytes = Encoding.UTF8.GetBytes(identifier);
        var identifierBase64 = Convert.ToBase64String(identifierBytes);
        
        var options = new PublicKeyCredentialCreationOptions()
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
        
        return options;
    }

    public static PublicKeyCredentialRequestOptions CreateRequestOptions(
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

        var options = new PublicKeyCredentialRequestOptions()
        {
            Challenge = challenge,
            Timeout = credentialOptions.Timeout,
            Domain = credentialOptions.Domain,
            UserVerification = UserVerifications.Required,
            AllowCredentials = allowCredentials
        };

        return options;
    }
    
    
}
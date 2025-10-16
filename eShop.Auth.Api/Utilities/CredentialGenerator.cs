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
        CredentialOptions credentialOptions)
    {
        var userIdBytes = user.Id.ToByteArray();
        var userIdBase64 = Convert.ToBase64String(userIdBytes);
        
        var options = new PublicKeyCredentialCreationOptions()
        {
            Challenge = challenge,
            PublicKeyCredentialUser = new PublicKeyCredentialUser
            {
                Id = userIdBase64,
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
        var allowedCredentials = new List<AllowedCredential>()
        {
            new AllowedCredential()
            {
                Type = KeyType.PublicKey,
                Id = passkey.CredentialId
            }
        };

        var options = new PublicKeyCredentialRequestOptions()
        {
            Challenge = challenge,
            Timeout = credentialOptions.Timeout,
            Domain = credentialOptions.Domain,
            UserVerification = UserVerifications.Required,
            AllowedCredentials = allowedCredentials
        };
        
        return options;
    }
    
    
}
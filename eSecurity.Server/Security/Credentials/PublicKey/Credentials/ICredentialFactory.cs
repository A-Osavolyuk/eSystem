using eSecurity.Core.Security.Credentials.PublicKey;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Credentials.PublicKey.Credentials;

public interface ICredentialFactory
{
    public PublicKeyCredentialCreationOptions CreateCreationOptions(
        UserEntity user, 
        string displayName, 
        string challenge,
        string fingerprint,
        CredentialOptions credentialOptions);

    public PublicKeyCredentialRequestOptions CreateRequestOptions(
        PasskeyEntity passkey,
        string challenge,
        CredentialOptions credentialOptions);
}
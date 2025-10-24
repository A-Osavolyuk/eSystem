using eSystem.Domain.Security.Credentials.PublicKey;

namespace eSystem.Auth.Api.Security.Credentials.PublicKey;

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
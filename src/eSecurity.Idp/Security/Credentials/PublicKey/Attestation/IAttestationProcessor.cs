using eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation;

public interface IAttestationProcessor
{
    ValueTask<TypedResult<AttestationVerificationResult>> ProcessAsync(AttestationObject attestationObject, 
        byte[] clientDataHash, CancellationToken cancellationToken = default);
}
using System.Security.Cryptography;
using eSecurity.Idp.Common.Validation;
using eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement;
using eSecurity.Idp.Security.Credentials.PublicKey.Credentials;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation;

public sealed class AttestationProcessor(
    IAttestationStatementParserProvider parserProvider,
    IAttestationStatementVerifierProvider verifierProvider,
    IOptions<CredentialOptions> options) : IAttestationProcessor
{
    private readonly IAttestationStatementParserProvider _parserProvider = parserProvider;
    private readonly IAttestationStatementVerifierProvider _verifierProvider = verifierProvider;
    private readonly CredentialOptions _credentialOptions = options.Value;

    public async ValueTask<TypedResult<AttestationVerificationResult>> ProcessAsync(
        AttestationObject attestationObject,
        byte[] clientDataHash,
        CancellationToken cancellationToken = default)
    {
        var authenticationData = attestationObject.AuthenticationData;
        var rpId = Normalizer.Normalize(_credentialOptions.Domain);
        var rpIdBytes = Encoding.UTF8.GetBytes(rpId);
        var rpHash = SHA256.HashData(rpIdBytes);
        if (!authenticationData.RpIdHash.SequenceEqual(rpHash))
        {
            return TypedResult<AttestationVerificationResult>.Fail(new Error
            {
                Code = ErrorCode.InvalidRp,
                Description = "Invalid RP ID"
            });
        }
        
        var parser = _parserProvider.GetParser(attestationObject.FormatType);
        
        var statement = parser.Parse(attestationObject.Statement);
        
        var verifier = _verifierProvider.GetVerifier(attestationObject.FormatType);
        
        var verificationResult = await verifier.VerifyAsync(attestationObject.AuthenticationData, 
            statement, clientDataHash, cancellationToken);

        return TypedResult<AttestationVerificationResult>.Success(verificationResult);
    }
}
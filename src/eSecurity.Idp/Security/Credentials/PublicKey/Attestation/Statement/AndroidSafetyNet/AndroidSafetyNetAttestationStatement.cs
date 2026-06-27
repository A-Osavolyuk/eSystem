namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.AndroidSafetyNet;

public sealed class AndroidSafetyNetAttestationStatement : IAttestationStatement
{
    public required string Response { get; init; }
}
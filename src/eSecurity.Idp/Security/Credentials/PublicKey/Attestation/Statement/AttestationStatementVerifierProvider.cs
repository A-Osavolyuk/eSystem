namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement;

public sealed class AttestationStatementVerifierProvider : IAttestationStatementVerifierProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<AttestationFormatType, Type> _verifiers;

    public AttestationStatementVerifierProvider(
        IServiceProvider serviceProvider, 
        IEnumerable<IAttestationStatementVerifier> verifiers)
    {
        _serviceProvider = serviceProvider;
        _verifiers = verifiers.ToDictionary(x => x.Format, x => x.GetType());
    }
    
    public IAttestationStatementVerifier GetVerifier(AttestationFormatType formatType)
    {
        if (!_verifiers.TryGetValue(formatType, out var verifierType))
            throw new InvalidOperationException("Invalid attestation format type");

        return (IAttestationStatementVerifier)_serviceProvider.GetRequiredService(verifierType);
    }
}
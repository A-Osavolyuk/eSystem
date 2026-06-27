namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement;

public sealed class AttestationStatementParserProvider : IAttestationStatementParserProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<AttestationFormatType, Type> _parsers;

    public AttestationStatementParserProvider(
        IServiceProvider serviceProvider, 
        IEnumerable<IAttestationStatementParser> parsers)
    {
        _serviceProvider = serviceProvider;
        _parsers = parsers.ToDictionary(x => x.FormatType, x => x.GetType());
    }
    
    public IAttestationStatementParser GetParser(AttestationFormatType formatType)
    {
        if (!_parsers.TryGetValue(formatType, out var parserType))
            throw new InvalidOperationException("Invalid attestation format type");

        return (IAttestationStatementParser)_serviceProvider.GetRequiredService(parserType);
    }
}
using System.Formats.Asn1;

namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.Tpm;

public sealed class TpmAttestation
{
    public byte[] Raw { get; init; } = null!;
    public byte[] ExtraData { get; init; } = null!;
    public byte[] PubArea { get; init; } = null!;
}

public static class TpmAttestationParser
{
    public static TpmAttestation Parse(byte[] certInfo)
    {
        var reader = new AsnReader(certInfo, AsnEncodingRules.DER);
        
        var seq = reader.ReadSequence();

        seq.ReadInteger();
        seq.ReadInteger();
        seq.ReadEncodedValue();

        var extraData = seq.ReadOctetString();
        var attested = seq.ReadEncodedValue();

        seq.ThrowIfNotEmpty();

        return new TpmAttestation
        {
            Raw = certInfo,
            ExtraData = extraData,
            PubArea = ExtractPubArea(attested)
        };
    }

    private static byte[] ExtractPubArea(ReadOnlyMemory<byte> attested)
    {
        var reader = new AsnReader(attested, AsnEncodingRules.DER);
        var seq = reader.ReadSequence();
        
        seq.ReadInteger();
        seq.ReadOctetString();
        
        return seq.ReadEncodedValue().ToArray();
    }
}
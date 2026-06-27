using eSystem.Core.Enums;
using Microsoft.AspNetCore.WebUtilities;
using PeterO.Cbor;

namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation;

public sealed class AttestationObject
{
    public required AuthenticationData AuthenticationData { get; init; }
    public required AttestationFormatType FormatType { get; init; }
    public required CBORObject Statement { get; init; }
    
    public static AttestationObject Parse(string attestationObject)
    {
        var bytes = WebEncoders.Base64UrlDecode(attestationObject);
        var cbor = CBORObject.DecodeFromBytes(bytes);
        
        var authData = AuthenticationData.Parse(cbor["authData"].GetByteString());
        var statement = cbor["attStmt"];
        
        if (!EnumHelper.TryParseFromString<AttestationFormatType>(cbor["fmt"].AsString(), out var fmt))
            throw new InvalidOperationException("Invalid FMT");

        return new AttestationObject
        {
            AuthenticationData = authData,
            FormatType = fmt.Value,
            Statement = statement
        };
    }
}
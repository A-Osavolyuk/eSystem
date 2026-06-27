using System.Buffers.Binary;
using System.Text.Json.Serialization;
using eSecurity.Idp.Security.Credentials.PublicKey.Attestation;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;
using Microsoft.AspNetCore.WebUtilities;
using PeterO.Cbor;

namespace eSecurity.Idp.Security.Credentials.PublicKey;

public sealed class AuthenticationData
{
    private byte Flags { get; set; }
    public byte[] RpIdHash { get; private set; } = null!;
    public byte[] Raw { get; private init; } = null!;
    public uint SignCount { get; private set; }
    public byte[] AaGuid { get; private set; } = null!;
    public byte[] CredentialId { get; private set; } = null!;
    public byte[] CredentialPublicKey { get; private set; } = null!;
    
    public static AuthenticationData Parse(byte[] authDataBytes)
    {
        var result = new AuthenticationData { Raw = authDataBytes };

        var span = authDataBytes.AsSpan();
        var offset = 0;

        // RP ID hash
        result.RpIdHash = span.Slice(offset, 32).ToArray();
        offset += 32;

        // Flags
        result.Flags = span[offset++];

        // Sign counter
        result.SignCount =
            BinaryPrimitives.ReadUInt32BigEndian(span.Slice(offset, 4));
        offset += 4;

        var hasAttestedCredentialData =
            (result.Flags & 0x40) != 0;

        if (hasAttestedCredentialData)
        {
            result.AaGuid =
                span.Slice(offset, 16).ToArray();
            offset += 16;

            var credIdLen =
                BinaryPrimitives.ReadUInt16BigEndian(span.Slice(offset, 2));
            offset += 2;

            result.CredentialId =
                span.Slice(offset, credIdLen).ToArray();
            offset += credIdLen;

            result.CredentialPublicKey =
                span[offset..].ToArray();
        }

        return result;
    }
}

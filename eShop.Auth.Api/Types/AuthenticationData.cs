using System.Buffers.Binary;
using PeterO.Cbor;

namespace eShop.Auth.Api.Types;

public class AuthenticationData
{
    public required byte[] RpIdHash { get; set; }
    public required byte Flags { get; set; }
    public required uint SignCount { get; set; }
    public required byte[] AaGuid { get; set; }
    public required byte[] CredentialId { get; set; }
    public required byte[] CredentialPublicKey { get; set; }

    public static AuthenticationData Parse(string attestationObject)
    {
        var attestationBytes = CredentialUtils.Base64UrlDecode(attestationObject);
        var attestationCbor = CBORObject.DecodeFromBytes(attestationBytes);
        var authDataBytes = attestationCbor["authData"].GetByteString();
        
        var offset = 0;

        // RP ID hash (32 bytes)
        var rpIdHash = authDataBytes.Skip(offset).Take(32).ToArray();
        offset += 32;

        // Flags (1 byte)
        var flags = authDataBytes[offset];
        offset += 1;

        // SignCount (4 bytes, big-endian)
        var signCount = BinaryPrimitives.ReadUInt32BigEndian(authDataBytes.AsSpan(offset, 4));
        offset += 4;

        // Credential data exists flag
        var hasCredentialData = (flags & 0x40) != 0;
        if (!hasCredentialData)
            throw new Exception("No credential data in authData");

        // AAGUID (16 bytes)
        var aaGuid = authDataBytes.Skip(offset).Take(16).ToArray();
        offset += 16;

        // Credential ID length (2 bytes, big-endian)
        var credentialIdLength = (authDataBytes[offset] << 8) | authDataBytes[offset + 1];
        offset += 2;

        // Credential ID
        var credentialId = authDataBytes.Skip(offset).Take(credentialIdLength).ToArray();
        offset += credentialIdLength;

        // Credential public key (remaining bytes)
        var credentialPublicKey = authDataBytes.Skip(offset).ToArray();

        return new AuthenticationData
        {
            RpIdHash = rpIdHash,
            Flags = flags,
            SignCount = signCount,
            AaGuid = aaGuid,
            CredentialId = credentialId,
            CredentialPublicKey = credentialPublicKey
        };
    }
}
using System.Buffers.Binary;
using eSystem.Core.Security.Cryptography.Encoding;
using Microsoft.AspNetCore.WebUtilities;
using PeterO.Cbor;

namespace eSecurity.Server.Security.Credentials.PublicKey;

public sealed class AuthenticationData
{
    public byte[] RpIdHash { get; set; } = null!;
    public byte Flags { get; set; }
    public uint SignCount { get; set; }
    public byte[] AaGuid { get; set; } = null!;
    public byte[] CredentialId { get; set; } = null!;
    public byte[] CredentialPublicKey { get; set; } = null!;
    public string? AttestationFormat { get; set; }
    public byte[]? AttestationSignature { get; set; }
    public byte[][]? AttestationCertificates { get; set; }
    public CBORObject? AttestationAdditionalData { get; set; }

    public static AuthenticationData Parse(string attestationObject)
    {
        var attestationBytes = WebEncoders.Base64UrlDecode(attestationObject);
        var attestationCbor = CBORObject.DecodeFromBytes(attestationBytes);
        var authDataBytes = attestationCbor["authData"].GetByteString();
        var authenticationData = new AuthenticationData();

        var offset = 0;

        // RP ID hash (32 bytes)
        authenticationData.RpIdHash = authDataBytes.Skip(offset).Take(32).ToArray();
        offset += 32;

        // Flags (1 byte)
        authenticationData.Flags = authDataBytes[offset];
        offset += 1;

        // SignCount (4 bytes, big-endian)
        var signCount = BinaryPrimitives.ReadUInt32BigEndian(authDataBytes.AsSpan(offset, 4));
        authenticationData.SignCount = signCount;
        offset += 4;

        // Credential data exists flag
        var hasCredentialData = (authenticationData.Flags & 0x40) != 0;
        if (!hasCredentialData)
            throw new Exception("No credential data in authData");

        // AAGUID (16 bytes)
        authenticationData.AaGuid = authDataBytes.Skip(offset).Take(16).ToArray();
        offset += 16;

        // Credential ID length (2 bytes, big-endian)
        var credentialIdLength = (authDataBytes[offset] << 8) | authDataBytes[offset + 1];
        offset += 2;

        // Credential ID
        authenticationData.CredentialId = authDataBytes.Skip(offset).Take(credentialIdLength).ToArray();
        offset += credentialIdLength;

        // Credential public key (remaining bytes in authData)
        authenticationData.CredentialPublicKey = authDataBytes.Skip(offset).ToArray();

        return attestationCbor.ContainsKey("attStmt") 
            ? ParseAttestationStatement(attestationCbor, authenticationData) 
            : authenticationData;
    }

    private static AuthenticationData ParseAttestationStatement(CBORObject attestationCbor, AuthenticationData authData)
    {
        var attStmt = attestationCbor["attStmt"];
        var fmt = attestationCbor["fmt"].AsString();
        byte[]? signature = null;
        byte[][]? x5Certificates = null;
        CBORObject? additionalData = null;

        switch (fmt)
        {
            case AttestationFormats.FidoU2F or AttestationFormats.Packed:
            {
                    
                if (attStmt.ContainsKey("sig"))
                    signature = attStmt["sig"].GetByteString();
                if (attStmt.ContainsKey("x5c"))
                    x5Certificates = attStmt["x5c"].Values.Select(c => c.GetByteString()).ToArray();
                break;
            }
            case AttestationFormats.Enterprise 
                or AttestationFormats.Apple 
                or AttestationFormats.AndroidKey 
                or AttestationFormats.Tpm:
            {
                    
                if (attStmt.ContainsKey("sig")) 
                    signature = attStmt["sig"].GetByteString();
                if (attStmt.ContainsKey("x5c"))
                    x5Certificates = attStmt["x5c"].Values.Select(c => c.GetByteString()).ToArray();
                additionalData = attStmt;
                break;
            }
            default:
            {
                additionalData = attStmt;
                break;
            }
        }

        authData.AttestationFormat = fmt;
        authData.AttestationSignature = signature;
        authData.AttestationCertificates = x5Certificates;
        authData.AttestationAdditionalData = additionalData;
        
        return authData;
    }

    private static class AttestationFormats
    {
        public const string Packed = "packed";
        public const string FidoU2F = "fido-u2f";
        public const string Enterprise = "enterprise";
        public const string Apple = "apple";
        public const string AndroidKey = "android-key";
        public const string Tpm = "tpm";
    }
}

using System.Buffers.Binary;
using System.Security.Cryptography;

namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.Tpm;

public static class TpmPubAreaParser
{
    public static TpmPublicKey Parse(ReadOnlySpan<byte> pubArea)
    {
        var offset = 0;

        // TPMI_ALG_PUBLIC (UINT16)
        var type = ReadUInt16(pubArea, ref offset);

        // nameAlg (UINT16)
        var nameAlg = ReadUInt16(pubArea, ref offset);

        // objectAttributes (UINT32)
        var objectAttributes = ReadUInt32(pubArea, ref offset);

        // authPolicy (TPM2B_DIGEST)
        var authPolicySize = ReadUInt16(pubArea, ref offset);
        offset += authPolicySize;

        return type switch
        {
            0x0001 => ParseRsa(pubArea, ref offset), // TPM_ALG_RSA
            0x0023 => ParseEc(pubArea, ref offset),  // TPM_ALG_ECC
            _ => throw new NotSupportedException($"Unsupported TPM alg: 0x{type:X4}")
        };
    }

    private static TpmPublicKey ParseRsa(ReadOnlySpan<byte> data, ref int offset)
    {
        var keyBits = ReadUInt16(data, ref offset);
        var exponent = ReadUInt32(data, ref offset);

        var modulusSize = ReadUInt16(data, ref offset);
        var modulus = data.Slice(offset, modulusSize).ToArray();
        offset += modulusSize;

        return new TpmPublicKey
        {
            Rsa = new RSAParameters
            {
                Exponent = exponent == 0 ? [0x01, 0x00, 0x01] : BitConverter.GetBytes(exponent).Reverse().ToArray(),
                Modulus = modulus
            }
        };
    }

    private static TpmPublicKey ParseEc(ReadOnlySpan<byte> data, ref int offset)
    {
        var curveId = ReadUInt16(data, ref offset);

        var xSize = ReadUInt16(data, ref offset);
        var x = data.Slice(offset, xSize).ToArray();
        offset += xSize;

        var ySize = ReadUInt16(data, ref offset);
        var y = data.Slice(offset, ySize).ToArray();
        offset += ySize;

        return new TpmPublicKey
        {
            Ec = new ECParameters
            {
                Curve = ECCurve.NamedCurves.nistP256,
                Q = new ECPoint { X = x, Y = y }
            }
        };
    }

    private static ushort ReadUInt16(ReadOnlySpan<byte> data, ref int offset)
    {
        var value = BinaryPrimitives.ReadUInt16BigEndian(data.Slice(offset, 2));
        offset += 2;
        return value;
    }

    private static uint ReadUInt32(ReadOnlySpan<byte> data, ref int offset)
    {
        var value = BinaryPrimitives.ReadUInt32BigEndian(data.Slice(offset, 4));
        offset += 4;
        return value;
    }
}
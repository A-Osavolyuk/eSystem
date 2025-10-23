using System.Security.Cryptography;
using eSystem.Domain.Security.Credentials.PublicKey;
using PeterO.Cbor;

namespace eSystem.Auth.Api.Security.Credentials.PublicKey;

public static class CredentialUtils
{
    public static byte[] Base64UrlDecode(string base64Url)
    {
        var padded = base64Url
            .Replace('-', '+')
            .Replace('_', '/');

        switch (padded.Length % 4)
        {
            case 2: padded += "=="; break;
            case 3: padded += "="; break;
        }

        return Convert.FromBase64String(padded);
    }

    public static string ToBase64String(string value)
    {
        var bytes = Base64UrlDecode(value);
        var base64 = Convert.ToBase64String(bytes);
        
        return base64;
    }

    public static uint ParseSignCount(string authenticatorData)
    {
        var authenticatorDataBytes = Base64UrlDecode(authenticatorData);
        var signCountBytes = authenticatorDataBytes.Skip(33).Take(4).ToArray();
        if (BitConverter.IsLittleEndian) Array.Reverse(signCountBytes);
        var signCount = BitConverter.ToUInt32(signCountBytes, 0);
        return signCount;
    }

    public static bool VerifySignature(AuthenticatorAssertionResponse response, byte[] publicKey)
    {
        var authenticatorDataBytes = Base64UrlDecode(response.AuthenticatorData);
        var signature = Base64UrlDecode(response.Signature);
        var clientDataJson = Base64UrlDecode(response.ClientDataJson);
        var clientDataHash = SHA256.HashData(clientDataJson);

        var signedData = authenticatorDataBytes.Concat(clientDataHash).ToArray();

        using var key = ImportCosePublicKey(publicKey);

        var valid = key switch
        {
            ECDsa ecdsa => ecdsa.VerifyData(signedData, signature, HashAlgorithmName.SHA256),
            RSA rsa => rsa.VerifyData(signedData, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1),
            _ => throw new NotSupportedException("Unsupported key type")
        };
        
        return valid;
    }
    
    private static AsymmetricAlgorithm ImportCosePublicKey(byte[] coseKey)
    {
        var obj = CBORObject.DecodeFromBytes(coseKey);

        var kty = obj[1].AsInt32();
        var alg = obj[3].AsInt32();

        switch (kty)
        {
            case 2:
                var crv = obj[-1].AsInt32();
                var x = obj[-2].GetByteString();
                var y = obj[-3].GetByteString();

                var ecdsa = ECDsa.Create(new ECParameters
                {
                    Curve = crv switch
                    {
                        1 => ECCurve.NamedCurves.nistP256,
                        2 => ECCurve.NamedCurves.nistP384,
                        3 => ECCurve.NamedCurves.nistP521,
                        _ => throw new NotSupportedException($"Unsupported curve: {crv}")
                    },
                    Q = new ECPoint { X = x, Y = y }
                });

                return ecdsa;

            case 3:
                var n = obj[-1].GetByteString();
                var e = obj[-2].GetByteString();

                var rsa = RSA.Create();
                rsa.ImportParameters(new RSAParameters
                {
                    Modulus = n,
                    Exponent = e
                });

                return rsa;

            default:
                throw new NotSupportedException($"Unsupported COSE key type: {kty}");
        }
    }
}
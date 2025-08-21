using System.Security.Cryptography;
using PeterO.Cbor;

namespace eShop.Auth.Api.Utilities;

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
    
    public static AsymmetricAlgorithm ImportCosePublicKey(byte[] coseKey)
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
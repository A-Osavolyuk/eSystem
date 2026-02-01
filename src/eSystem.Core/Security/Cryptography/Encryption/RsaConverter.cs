using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace eSystem.Core.Security.Cryptography.Encryption;

public static class RsaConverter
{
    public static RSA FromJsonWebkey(JsonWebKey jwk)
    {
        var rsa = RSA.Create();

        var parameters = new RSAParameters
        {
            Modulus = Base64UrlEncoder.DecodeBytes(jwk.N),
            Exponent = Base64UrlEncoder.DecodeBytes(jwk.E)
        };

        rsa.ImportParameters(parameters);
        return rsa;
    }
}
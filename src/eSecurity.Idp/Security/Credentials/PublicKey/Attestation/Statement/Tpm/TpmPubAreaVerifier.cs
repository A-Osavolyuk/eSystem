using System.Security.Cryptography.X509Certificates;

namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation.Statement.Tpm;

public static class TpmPubAreaVerifier
{
    public static bool Verify(byte[] pubArea, X509Certificate2 cert)
    {
        var tpmKey = TpmPubAreaParser.Parse(pubArea);

        if (tpmKey.IsRsa)
        {
            using var rsa = cert.GetRSAPublicKey();
            if (rsa == null) return false;

            var certParams = rsa.ExportParameters(false);

            return certParams.Modulus!.SequenceEqual(tpmKey.Rsa!.Value.Modulus) &&
                   certParams.Exponent!.SequenceEqual(tpmKey.Rsa!.Value.Exponent);
        }

        if (tpmKey.IsEc)
        {
            using var ec = cert.GetECDsaPublicKey();
            if (ec == null) return false;

            var certParams = ec.ExportParameters(false);
            var tpmEc = tpmKey.Ec!.Value;

            return certParams.Q.X!.SequenceEqual(tpmEc.Q.X) &&
                   certParams.Q.Y!.SequenceEqual(tpmEc.Q.Y);
        }

        return false;
    }
}
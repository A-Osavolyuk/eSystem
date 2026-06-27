using System.Security.Cryptography.X509Certificates;

namespace eSecurity.Idp.Security.Cryptography.Certificates;

public static class CertificateTrustValidator
{
    public static bool Validate(X509Certificate2 cert)
    {
        using var chain = new X509Chain();

        chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;

        return chain.Build(cert);
    }
}
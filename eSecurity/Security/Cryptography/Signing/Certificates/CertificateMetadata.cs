using System.Security.Cryptography.X509Certificates;

namespace eSecurity.Security.Cryptography.Signing.Certificates;

public class CertificateMetadata
{
    public required byte[] ProtectedPassword { get; set; }
    public required byte[] ProtectedCertificate { get; set; }
    public required X509Certificate2 Certificate { get; set; }
}
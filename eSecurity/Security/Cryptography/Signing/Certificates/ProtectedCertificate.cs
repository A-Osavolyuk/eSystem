using System.Security.Cryptography.X509Certificates;

namespace eSecurity.Security.Cryptography.Signing.Certificates;

public class ProtectedCertificate
{
    public required X509Certificate2 Certificate { get; set; }
    public required byte[] ProtectedPasswordBytes { get; set; }
    public required byte[] ProtectedCertificateBytes { get; set; }
}
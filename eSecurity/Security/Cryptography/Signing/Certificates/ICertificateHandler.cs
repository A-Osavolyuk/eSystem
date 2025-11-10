using System.Security.Cryptography.X509Certificates;

namespace eSecurity.Security.Cryptography.Signing.Certificates;

public interface ICertificateHandler
{
    public CertificateMetadata CreateCertificate();
    public X509Certificate2 ExportCertificate(byte[] protectedCertificate, byte[] protectedPassword);
}
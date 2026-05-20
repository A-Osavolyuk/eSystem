using System.Security.Cryptography.X509Certificates;

namespace eSecurity.Idp.Security.Cryptography.Signing.Certificates;

public interface ICertificateHandler
{
    public ProtectedCertificate CreateCertificate();
    public X509Certificate2 ExportCertificate(byte[] protectedCertificate, byte[] protectedPassword);
}
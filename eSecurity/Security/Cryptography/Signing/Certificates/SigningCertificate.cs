using System.Security.Cryptography.X509Certificates;

namespace eSecurity.Security.Cryptography.Signing.Certificates;

public sealed class SigningCertificate
{
    public required Guid Id { get; set; }
    public required X509Certificate2 Certificate { get; set; }
}
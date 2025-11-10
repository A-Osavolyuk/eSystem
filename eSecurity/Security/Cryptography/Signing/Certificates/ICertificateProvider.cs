using eSecurity.Security.Cryptography.Signing.Certificates;

namespace eSecurity.Security.Cryptography.Signing.Keys;

public interface ICertificateProvider
{
    public ValueTask<SigningCertificate> GetCertificateAsync(CancellationToken cancellationToken = default);
}
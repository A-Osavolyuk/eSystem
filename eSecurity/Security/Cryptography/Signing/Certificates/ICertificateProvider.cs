namespace eSecurity.Security.Cryptography.Signing.Certificates;

public interface ICertificateProvider
{
    public ValueTask<SigningCertificate> GetCertificateAsync(CancellationToken cancellationToken = default);
}
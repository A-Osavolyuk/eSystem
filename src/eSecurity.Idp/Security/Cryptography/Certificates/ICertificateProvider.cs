namespace eSecurity.Idp.Security.Cryptography.Certificates;

public interface ICertificateProvider
{
    public ValueTask<SigningCertificate?> FindByIdAsync(Guid kid, CancellationToken cancellationToken = default);
    public ValueTask<SigningCertificate> GetActiveAsync(CancellationToken cancellationToken = default);
}
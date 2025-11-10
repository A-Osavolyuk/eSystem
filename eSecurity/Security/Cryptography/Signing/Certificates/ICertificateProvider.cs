using System.Security.Claims;

namespace eSecurity.Security.Cryptography.Signing.Certificates;

public interface ICertificateProvider
{
    public ValueTask<SigningCertificate?> FindByIdAsync(Guid kid, CancellationToken cancellationToken = default);
    public ValueTask<SigningCertificate> GetActiveAsync(CancellationToken cancellationToken = default);
    public ValueTask<List<SigningCertificate>> GetValidAsync(CancellationToken cancellationToken = default);
}
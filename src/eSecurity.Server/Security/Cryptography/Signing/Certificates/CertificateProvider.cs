using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Cryptography.Signing.Certificates;

public class CertificateProvider(
    AuthDbContext context,
    ICertificateHandler certificateHandler) : ICertificateProvider
{
    private readonly AuthDbContext _context = context;
    private readonly ICertificateHandler _certificateHandler = certificateHandler;

    public async ValueTask<SigningCertificate?> FindByIdAsync(Guid kid, CancellationToken cancellationToken = default)
    {
        var certificateEntity = await _context.Certificates.SingleOrDefaultAsync(
            x => x.Id == kid, cancellationToken);

        if (certificateEntity is null) return null;

        var certificate = _certificateHandler.ExportCertificate(
            certificateEntity.ProtectedCertificate,
            certificateEntity.ProtectedPassword
        );

        return new SigningCertificate
        {
            Id = certificateEntity.Id,
            Certificate = certificate
        };
    }

    public async ValueTask<SigningCertificate> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var certificateEntity = await _context.Certificates.SingleOrDefaultAsync(
            x => x.IsActive, cancellationToken);

        if (certificateEntity is null)
        {
            var protectedCertificate = _certificateHandler.CreateCertificate();

            var entity = new SigningCertificateEntity
            {
                Id = Guid.CreateVersion7(),
                IsActive = true,
                ProtectedPassword = protectedCertificate.ProtectedPasswordBytes,
                ProtectedCertificate = protectedCertificate.ProtectedCertificateBytes,
                ExpireDate = protectedCertificate.Certificate.NotAfter.ToUniversalTime()
            };

            await _context.Certificates.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new SigningCertificate
            {
                Id = entity.Id,
                Certificate = protectedCertificate.Certificate
            };
        }

        var certificate = _certificateHandler.ExportCertificate(
            certificateEntity.ProtectedCertificate,
            certificateEntity.ProtectedPassword);

        return new SigningCertificate
        {
            Id = certificateEntity.Id,
            Certificate = certificate
        };
    }
}
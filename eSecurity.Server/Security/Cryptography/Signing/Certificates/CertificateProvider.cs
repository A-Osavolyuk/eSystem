using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Cryptography.Signing.Certificates;

public class CertificateProvider(
    AuthDbContext context,
    ICertificateHandler certificateHandler) : ICertificateProvider
{
    private readonly AuthDbContext context = context;
    private readonly ICertificateHandler certificateHandler = certificateHandler;

    public async ValueTask<SigningCertificate?> FindByIdAsync(Guid kid, CancellationToken cancellationToken = default)
    {
        var certificateEntity = await context.Certificates.SingleOrDefaultAsync(
            x => x.Id == kid, cancellationToken);

        if (certificateEntity is null) return null;

        var certificate = certificateHandler.ExportCertificate(
            certificateEntity.ProtectedCertificate,
            certificateEntity.ProtectedPassword
        );

        return new SigningCertificate()
        {
            Id = certificateEntity.Id,
            Certificate = certificate
        };
    }

    public async ValueTask<SigningCertificate> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var certificateEntity = await context.Certificates.SingleOrDefaultAsync(
            x => x.IsActive, cancellationToken);

        if (certificateEntity is null)
        {
            var protectedCertificate = certificateHandler.CreateCertificate();

            var entity = new SigningCertificateEntity()
            {
                Id = Guid.CreateVersion7(),
                IsActive = true,
                ProtectedPassword = protectedCertificate.ProtectedPasswordBytes,
                ProtectedCertificate = protectedCertificate.ProtectedCertificateBytes,
                ExpireDate = protectedCertificate.Certificate.NotAfter,
                CreateDate = DateTimeOffset.UtcNow,
            };

            await context.Certificates.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return new SigningCertificate()
            {
                Id = entity.Id,
                Certificate = protectedCertificate.Certificate
            };
        }

        var certificate = certificateHandler.ExportCertificate(
            certificateEntity.ProtectedCertificate,
            certificateEntity.ProtectedPassword);

        return new SigningCertificate()
        {
            Id = certificateEntity.Id,
            Certificate = certificate
        };
    }

    public async ValueTask<List<SigningCertificate>> GetValidAsync(CancellationToken cancellationToken = default)
    {
        return await context.Certificates
            .Where(x => x.IsValid)
            .Select(certificate => new SigningCertificate()
            {
                Id = certificate.Id,
                Certificate = certificateHandler.ExportCertificate(
                    certificate.ProtectedCertificate,
                    certificate.ProtectedPassword
                )
            })
            .ToListAsync(cancellationToken);
    }
}
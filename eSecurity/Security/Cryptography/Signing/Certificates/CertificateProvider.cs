using eSecurity.Data.Entities;

namespace eSecurity.Security.Cryptography.Signing.Certificates;

public class CertificateProvider(
    AuthDbContext context,
    ICertificateHandler certificateHandler) : ICertificateProvider
{
    private readonly AuthDbContext context = context;
    private readonly ICertificateHandler certificateHandler = certificateHandler;

    public async ValueTask<SigningCertificate> GetCertificateAsync(CancellationToken cancellationToken = default)
    {
        var certificateEntity = await context.Certificates.SingleOrDefaultAsync(
            x => x.IsActive, cancellationToken);
        
        if (certificateEntity is null)
        {
            var protectedCertificate = certificateHandler.CreateCertificate();

            var entity = new SigningCertificateEntity()
            {
                Id = Guid.CreateVersion7(),
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
}
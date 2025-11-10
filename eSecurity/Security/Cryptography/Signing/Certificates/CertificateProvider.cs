using System.Security.Cryptography.X509Certificates;
using eSecurity.Data.Entities;
using eSecurity.Security.Cryptography.Signing.Certificates;

namespace eSecurity.Security.Cryptography.Signing.Keys;

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
            var signingCertificate = certificateHandler.CreateCertificate();

            var entity = new SigningCertificateEntity()
            {
                Id = Guid.CreateVersion7(),
                ProtectedPassword = signingCertificate.ProtectedPassword,
                ProtectedCertificate = signingCertificate.ProtectedCertificate,
                ExpireDate = signingCertificate.Certificate.NotAfter,
                CreateDate = DateTimeOffset.UtcNow,
            };

            await context.Certificates.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return new SigningCertificate()
            {
                Id = entity.Id,
                Certificate = signingCertificate.Certificate
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
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using eSecurity.Data.Entities;
using eSecurity.Security.Cryptography.Protection;
using eSystem.Core.Security.Cryptography.Keys;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Security.Cryptography.Keys.SigningKey;

public class SigningKeyProvider(
    AuthDbContext context,
    IKeyFactory keyFactory,
    IDataProtectionProvider protection,
    IOptions<SigningKeyOptions> options) : ISigningKeyProvider
{
    private readonly AuthDbContext context = context;
    private readonly IKeyFactory keyFactory = keyFactory;
    private readonly SigningKeyOptions options = options.Value;
    private readonly IDataProtector passwordProtector = protection.CreateProtector(ProtectionPurposes.Password);
    private readonly IDataProtector certificateProtector = protection.CreateProtector(ProtectionPurposes.Password);

    public async Task<SigningKey> GetAsync(CancellationToken cancellationToken = default)
    {
        var key = await context.SigningKeys.SingleOrDefaultAsync(x => x.IsActive, cancellationToken);

        if (key is null)
        {
            var rsa = RSA.Create(options.KeyLength * 8);

            var request = new CertificateRequest(
                options.SubjectName,
                rsa,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1
            );
            
            var password = keyFactory.Create(20);
            var protectedPassword = passwordProtector.Protect(Encoding.UTF8.GetBytes(password));
            
            var certificate = request.CreateSelfSigned(
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow.Add(options.CertificateLifetime));
            
            var certificateBytes = certificate.Export(X509ContentType.Pkcs12, password);
            var protectedCertificate = certificateProtector.Protect(certificateBytes);

            key = new SigningKeyEntity()
            {
                Id = Guid.CreateVersion7(),
                ProtectedPassword = protectedPassword,
                ProtectedPfx = protectedCertificate,
                ExpireDate = DateTimeOffset.UtcNow.Add(options.CertificateLifetime),
                CreateDate = DateTimeOffset.UtcNow,
            };
            
            await context.SigningKeys.AddAsync(key, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return new SigningKey()
            {
                Id = key.Id,
                PublicKey = certificate.GetRSAPublicKey()!,
                PrivateKey = certificate.GetRSAPrivateKey()!
            };
        }
        else
        {
            var passwordBytes = passwordProtector.Unprotect(key.ProtectedPassword);
            var password = Encoding.UTF8.GetString(passwordBytes);
            var pfxBytes = certificateProtector.Unprotect(key.ProtectedPfx);
            
            var certificate = X509CertificateLoader.LoadPkcs12(
                pfxBytes,
                password,
                X509KeyStorageFlags.EphemeralKeySet | X509KeyStorageFlags.Exportable);
            
            return new SigningKey()
            {
                Id = key.Id,
                PublicKey = certificate.GetRSAPublicKey()!,
                PrivateKey = certificate.GetRSAPrivateKey()!
            };
        }
    }
}
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using eSecurity.Security.Cryptography.Keys;
using eSecurity.Security.Cryptography.Protection;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Security.Cryptography.Signing.Certificates;

public class CertificateHandler(
    IDataProtectionProvider protection,
    IKeyFactory keyFactory,
    IOptions<CertificateOptions> options) : ICertificateHandler
{
    private readonly IKeyFactory keyFactory = keyFactory;
    private readonly CertificateOptions options = options.Value;
    private readonly IDataProtector passwordProtector = protection.CreateProtector(ProtectionPurposes.Password);
    private readonly IDataProtector certificateProtector = protection.CreateProtector(ProtectionPurposes.Certificate);
    
    public CertificateMetadata CreateCertificate()
    {
        using var rsa = RSA.Create(options.KeyLength * 8);

        var request = new CertificateRequest(
            options.SubjectName,
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1
        );
        
        var certificate = request.CreateSelfSigned(
            DateTimeOffset.UtcNow.Subtract(TimeSpan.FromMinutes(1)),
            DateTimeOffset.UtcNow.Add(options.CertificateLifetime));

        var password = keyFactory.Create(20);
        var certificateBytes = certificate.Export(X509ContentType.Pkcs12, password);

        return new CertificateMetadata()
        {
            ProtectedCertificate = certificateProtector.Protect(certificateBytes),
            ProtectedPassword = passwordProtector.Protect(Encoding.UTF8.GetBytes(password)),
            Certificate = certificate
        };
    }

    public X509Certificate2 ExportCertificate(byte[] protectedCertificate, byte[] protectedPassword)
    {
        var passwordBytes = passwordProtector.Unprotect(protectedPassword);
        var password = Encoding.UTF8.GetString(passwordBytes);
        var certificateBytes = certificateProtector.Unprotect(protectedCertificate);
        
        try
        {
            return X509CertificateLoader.LoadPkcs12(
                certificateBytes,
                password,
                X509KeyStorageFlags.EphemeralKeySet | X509KeyStorageFlags.Exportable);
        }
        catch (CryptographicException ex)
        {
            throw new InvalidOperationException("Failed to load certificate.", ex);
        }
    }
}